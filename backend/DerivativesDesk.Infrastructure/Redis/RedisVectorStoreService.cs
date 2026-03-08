using System.Text;
using DerivativesDesk.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DerivativesDesk.Infrastructure.Redis;

/// <summary>
/// Redis Stack vector store using raw FT.CREATE / FT.SEARCH commands
/// since NRediSearch does not expose vector field APIs.
/// </summary>
public class RedisVectorStoreService : IVectorStoreService
{
    private const string IndexName = "derivatives-idx";
    private const string KeyPrefix = "doc:";
    private const int VectorDim = 1536; // OpenAI text-embedding-3-small

    private readonly IDatabase _db;
    private readonly ILogger<RedisVectorStoreService> _logger;

    public RedisVectorStoreService(IConfiguration configuration, ILogger<RedisVectorStoreService> logger)
    {
        _logger = logger;
        var connStr = configuration["REDIS_CONNECTION_STRING"] ?? "localhost:6379";
        var mux = ConnectionMultiplexer.Connect(connStr);
        _db = mux.GetDatabase();
    }

    public async Task EnsureIndexAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _db.ExecuteAsync("FT.INFO", IndexName);
            _logger.LogInformation("Redis index '{Index}' already exists.", IndexName);
        }
        catch (RedisServerException ex) when (ex.Message.Contains("Unknown index name"))
        {
            _logger.LogInformation("Creating Redis index '{Index}' (HNSW, DIM={Dim})...", IndexName, VectorDim);

            // FT.CREATE derivatives-idx ON HASH PREFIX 1 doc:
            //   SCHEMA text TEXT source TAG contract_id TAG created_at NUMERIC
            //          embedding VECTOR HNSW 6 TYPE FLOAT32 DIM 1536 DISTANCE_METRIC COSINE
            await _db.ExecuteAsync("FT.CREATE", [
                IndexName,
                "ON", "HASH",
                "PREFIX", "1", KeyPrefix,
                "SCHEMA",
                "text",        "TEXT",
                "source",      "TAG",
                "contract_id", "TAG",
                "created_at",  "NUMERIC",
                "embedding",   "VECTOR", "HNSW", "6",
                               "TYPE", "FLOAT32",
                               "DIM", VectorDim.ToString(),
                               "DISTANCE_METRIC", "COSINE"
            ]);

            _logger.LogInformation("Redis index '{Index}' created.", IndexName);
        }
    }

    public async Task UpsertAsync(VectorDocument document, CancellationToken cancellationToken = default)
    {
        var key = $"{KeyPrefix}{document.Id}";
        await _db.HashSetAsync(key,
        [
            new HashEntry("text",        document.Text),
            new HashEntry("source",      document.Source),
            new HashEntry("contract_id", document.ContractId ?? string.Empty),
            new HashEntry("created_at",  DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            new HashEntry("embedding",   VectorToBytes(document.Embedding))
        ]);
    }

    public async Task UpsertBatchAsync(IEnumerable<VectorDocument> documents, CancellationToken cancellationToken = default)
    {
        var batch = _db.CreateBatch();
        var tasks = new List<Task>();
        var unixNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        foreach (var doc in documents)
        {
            var key = $"{KeyPrefix}{doc.Id}";
            tasks.Add(batch.HashSetAsync(key,
            [
                new HashEntry("text",        doc.Text),
                new HashEntry("source",      doc.Source),
                new HashEntry("contract_id", doc.ContractId ?? string.Empty),
                new HashEntry("created_at",  unixNow),
                new HashEntry("embedding",   VectorToBytes(doc.Embedding))
            ]));
        }

        batch.Execute();
        await Task.WhenAll(tasks);
    }

    public async Task<IReadOnlyList<VectorSearchResult>> SearchAsync(
        float[] queryEmbedding, int topK = 7, string? sourceFilter = null, CancellationToken cancellationToken = default)
    {
        var queryBytes = VectorToBytes(queryEmbedding);

        // KNN query with optional tag filter
        var queryStr = sourceFilter != null
            ? $"@source:{{{sourceFilter}}}=>[KNN {topK} @embedding $vec AS score]"
            : $"*=>[KNN {topK} @embedding $vec AS score]";

        // FT.SEARCH derivatives-idx <query> PARAMS 2 vec <bytes> RETURN 3 text source score SORTBY score DIALECT 2
        var args = new List<object>
        {
            IndexName, queryStr,
            "PARAMS", "2", "vec", queryBytes,
            "RETURN", "3", "text", "source", "score",
            "SORTBY", "score",
            "LIMIT", "0", topK.ToString(),
            "DIALECT", "2"
        };

        var result = await _db.ExecuteAsync("FT.SEARCH", args.ToArray());
        return ParseSearchResults(result);
    }

    public async Task DeleteBySourceAsync(string source, CancellationToken cancellationToken = default)
    {
        // FT.SEARCH to find all keys with this source tag
        var result = await _db.ExecuteAsync("FT.SEARCH",
            IndexName, $"@source:{{{source}}}", "LIMIT", "0", "10000", "RETURN", "0");

        var parsed = (RedisResult[])result!;
        var count = (long)parsed[0];
        if (count == 0) return;

        var keys = new List<RedisKey>();
        for (int i = 1; i < parsed.Length; i += 2)
            keys.Add((string)parsed[i]!);

        if (keys.Count > 0)
        {
            await _db.KeyDeleteAsync(keys.ToArray());
            _logger.LogInformation("Deleted {Count} vectors with source='{Source}'.", keys.Count, source);
        }
    }

    private static IReadOnlyList<VectorSearchResult> ParseSearchResults(RedisResult result)
    {
        var results = new List<VectorSearchResult>();
        var array = (RedisResult[])result!;
        // array[0] = total count; then pairs: [key, [field, value, ...]]
        for (int i = 1; i < array.Length; i += 2)
        {
            var id = (string)array[i]!;
            var fields = (RedisResult[])array[i + 1]!;

            string text = string.Empty, source = string.Empty;
            double score = 0;

            for (int j = 0; j < fields.Length - 1; j += 2)
            {
                var name = (string)fields[j]!;
                var value = (string)fields[j + 1]!;
                switch (name)
                {
                    case "text":   text   = value; break;
                    case "source": source = value; break;
                    case "score":  double.TryParse(value, out score); break;
                }
            }

            results.Add(new VectorSearchResult(id, text, source, score));
        }
        return results;
    }

    private static byte[] VectorToBytes(float[] vector)
    {
        var bytes = new byte[vector.Length * sizeof(float)];
        Buffer.BlockCopy(vector, 0, bytes, 0, bytes.Length);
        return bytes;
    }
}

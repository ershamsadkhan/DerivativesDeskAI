namespace DerivativesDesk.Core.Interfaces;

public record VectorDocument(
    string Id,
    string Text,
    float[] Embedding,
    string Source,
    string? ContractId = null);

public record VectorSearchResult(string Id, string Text, string Source, double Score);

public interface IVectorStoreService
{
    Task EnsureIndexAsync(CancellationToken cancellationToken = default);
    Task UpsertAsync(VectorDocument document, CancellationToken cancellationToken = default);
    Task UpsertBatchAsync(IEnumerable<VectorDocument> documents, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VectorSearchResult>> SearchAsync(float[] queryEmbedding, int topK = 7, string? sourceFilter = null, CancellationToken cancellationToken = default);
    Task DeleteBySourceAsync(string source, CancellationToken cancellationToken = default);
}

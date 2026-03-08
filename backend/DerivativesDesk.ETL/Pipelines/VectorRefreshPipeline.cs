using DerivativesDesk.AI.Services;
using DerivativesDesk.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DerivativesDesk.ETL.Pipelines;

public class VectorRefreshPipeline(
    IFuturesRepository futuresRepo,
    IRolloverRepository rolloverRepo,
    IOrdersRepository ordersRepo,
    IEmbeddingService embeddingService,
    IVectorStoreService vectorStore,
    ILogger<VectorRefreshPipeline> logger) : IEtlPipeline
{
    public async Task<EtlResult> RunAsync(CancellationToken cancellationToken = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        logger.LogInformation("Starting vector DB refresh pipeline...");

        try
        {
            await vectorStore.EnsureIndexAsync(cancellationToken);

            var contractsCount   = await ProcessFuturesContractsAsync(cancellationToken);
            var rolloversCount   = await ProcessRolloversAsync(cancellationToken);
            var ordersCount      = await ProcessOrdersAsync(cancellationToken);

            sw.Stop();
            logger.LogInformation(
                "ETL complete: {Contracts} contracts, {Rollovers} rollovers, {Orders} orders in {Elapsed}ms",
                contractsCount, rolloversCount, ordersCount, sw.ElapsedMilliseconds);

            return new EtlResult(contractsCount, rolloversCount, ordersCount, sw.Elapsed);
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "ETL pipeline failed after {Elapsed}ms", sw.ElapsedMilliseconds);
            return new EtlResult(0, 0, 0, sw.Elapsed, ex.Message);
        }
    }

    private async Task<int> ProcessFuturesContractsAsync(CancellationToken ct)
    {
        logger.LogInformation("Processing futures contracts...");
        await vectorStore.DeleteBySourceAsync("futures_contract", ct);

        var contracts = (await futuresRepo.GetAllActiveAsync(ct)).ToList();
        var texts = contracts.Select(c => c.ToTextChunk()).ToList();
        var embeddings = await embeddingService.EmbedBatchAsync(texts, ct);

        var docs = contracts.Zip(embeddings, (c, e) => new VectorDocument(
            Id:         $"futures_{c.ContractId}",
            Text:       c.ToTextChunk(),
            Embedding:  e,
            Source:     "futures_contract",
            ContractId: c.ContractId.ToString()));

        await vectorStore.UpsertBatchAsync(docs, ct);
        logger.LogInformation("Upserted {Count} futures contract vectors.", contracts.Count);
        return contracts.Count;
    }

    private async Task<int> ProcessRolloversAsync(CancellationToken ct)
    {
        logger.LogInformation("Processing rollover periods...");
        await vectorStore.DeleteBySourceAsync("rollover", ct);

        var rollovers = (await rolloverRepo.GetAllAsync(ct)).ToList();
        var texts = rollovers.Select(r => r.ToTextChunk()).ToList();
        var embeddings = await embeddingService.EmbedBatchAsync(texts, ct);

        var docs = rollovers.Zip(embeddings, (r, e) => new VectorDocument(
            Id:        $"rollover_{r.RolloverId}",
            Text:      r.ToTextChunk(),
            Embedding: e,
            Source:    "rollover"));

        await vectorStore.UpsertBatchAsync(docs, ct);
        logger.LogInformation("Upserted {Count} rollover period vectors.", rollovers.Count);
        return rollovers.Count;
    }

    private async Task<int> ProcessOrdersAsync(CancellationToken ct)
    {
        logger.LogInformation("Processing orders (last 90 days)...");
        await vectorStore.DeleteBySourceAsync("order", ct);

        var orders = (await ordersRepo.GetRecentAsync(90, ct)).ToList();
        var texts = orders.Select(o => o.ToTextChunk()).ToList();
        var embeddings = await embeddingService.EmbedBatchAsync(texts, ct);

        var docs = orders.Zip(embeddings, (o, e) => new VectorDocument(
            Id:         $"order_{o.OrderId}",
            Text:       o.ToTextChunk(),
            Embedding:  e,
            Source:     "order",
            ContractId: o.ContractId.ToString()));

        await vectorStore.UpsertBatchAsync(docs, ct);
        logger.LogInformation("Upserted {Count} order vectors.", orders.Count);
        return orders.Count;
    }
}

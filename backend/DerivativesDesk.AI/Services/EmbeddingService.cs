using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Embeddings;

namespace DerivativesDesk.AI.Services;

public interface IEmbeddingService
{
    Task<float[]> EmbedAsync(string text, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<float[]>> EmbedBatchAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default);
}

public class EmbeddingService(
    ITextEmbeddingGenerationService embeddingGenerator,
    ILogger<EmbeddingService> logger) : IEmbeddingService
{
    public async Task<float[]> EmbedAsync(string text, CancellationToken cancellationToken = default)
    {
        var result = await embeddingGenerator.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);
        return result.ToArray();
    }

    public async Task<IReadOnlyList<float[]>> EmbedBatchAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        var textList = texts.ToList();
        logger.LogInformation("Embedding batch of {Count} texts...", textList.Count);

        var results = await embeddingGenerator.GenerateEmbeddingsAsync(textList, cancellationToken: cancellationToken);
        return results.Select(r => r.ToArray()).ToList();
    }
}

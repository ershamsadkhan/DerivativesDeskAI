using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Anthropic;
using Anthropic.Models.Messages;
using DerivativesDesk.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DerivativesDesk.AI.Services;

public class ChatService(
    AnthropicClient anthropicClient,
    IEmbeddingService embeddingService,
    IVectorStoreService vectorStore,
    ILogger<ChatService> logger) : IChatService
{
    private static readonly ConcurrentDictionary<string, List<(string Role, string Content)>> _sessions = new();

    private const string ModelId  = "claude-sonnet-4-6";
    private const int MaxTokens   = 2048;
    private const string SystemPrompt = """
        You are an AI assistant for a financial derivatives trading desk.
        You have access to real-time data on futures contracts, rollover periods, and orders
        (placed, filled, cancelled) across equity indices, rates, energy, metals, and FX.

        Guidelines:
        - Answer questions about futures contracts, their expiry dates, lot sizes, and exchanges
        - Explain rollover periods and which contracts are rolling or upcoming
        - Summarize order activity by status, PM, contract, or date range
        - Be precise with dates, symbols, and financial terminology
        - If data is not in the provided context, say so clearly — do not fabricate
        - Format numbers clearly (prices with 4 decimal places, quantities as integers)
        - Keep responses concise and factual; use bullet points for lists
        """;

    public async IAsyncEnumerable<string> StreamAsync(
        string sessionId,
        string userMessage,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // 1. Embed query
        var queryEmbedding = await embeddingService.EmbedAsync(userMessage, cancellationToken);

        // 2. Retrieve context from Redis
        var searchResults = await vectorStore.SearchAsync(queryEmbedding, topK: 7, cancellationToken: cancellationToken);
        var context = searchResults.Count > 0
            ? string.Join("\n", searchResults.Select(r => r.Text))
            : "No relevant data found in the vector store.";

        logger.LogInformation("Retrieved {Count} context chunks for session {SessionId}.", searchResults.Count, sessionId);

        // 3. Augment message with context
        var augmentedMessage = $"""
            Context from derivatives database:
            {context}

            User question: {userMessage}
            """;

        // 4. Manage session history (keep last 20 turns to avoid token overflow)
        var history = _sessions.GetOrAdd(sessionId, _ => []);
        history.Add(("user", userMessage));

        // 5. Build Anthropic message list
        var messagesToSend = history
            .SkipLast(1)                          // exclude current (added above)
            .TakeLast(18)                          // keep last 18 prior turns
            .Select(h => new MessageParam
            {
                Role    = h.Role == "user" ? Role.User : Role.Assistant,
                Content = h.Content
            })
            .ToList();

        messagesToSend.Add(new MessageParam
        {
            Role    = Role.User,
            Content = augmentedMessage
        });

        // 6. Stream from Claude
        var fullResponse = new System.Text.StringBuilder();

        var streamParams = new MessageCreateParams
        {
            Model      = ModelId,
            MaxTokens  = MaxTokens,
            System     = SystemPrompt,
            Messages   = messagesToSend
        };

        await foreach (var streamEvent in anthropicClient.Messages.CreateStreaming(streamParams, cancellationToken))
        {
            if (streamEvent.TryPickContentBlockDelta(out var blockDelta)
                && blockDelta.Delta.TryPickText(out var textDelta)
                && !string.IsNullOrEmpty(textDelta.Text))
            {
                fullResponse.Append(textDelta.Text);
                yield return textDelta.Text;
            }
        }

        // 7. Persist assistant response
        history.Add(("assistant", fullResponse.ToString()));
    }

    public Task<IReadOnlyList<(string Role, string Content)>> GetHistoryAsync(string sessionId)
    {
        var history = _sessions.GetOrAdd(sessionId, _ => []);
        IReadOnlyList<(string, string)> result = history.AsReadOnly();
        return Task.FromResult(result);
    }
}

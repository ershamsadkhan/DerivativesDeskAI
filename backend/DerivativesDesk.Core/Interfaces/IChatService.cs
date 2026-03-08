namespace DerivativesDesk.Core.Interfaces;

public interface IChatService
{
    IAsyncEnumerable<string> StreamAsync(string sessionId, string userMessage, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(string Role, string Content)>> GetHistoryAsync(string sessionId);
}

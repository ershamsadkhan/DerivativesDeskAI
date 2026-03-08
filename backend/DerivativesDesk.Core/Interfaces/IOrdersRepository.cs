using DerivativesDesk.Core.Models;

namespace DerivativesDesk.Core.Interfaces;

public interface IOrdersRepository
{
    Task<IEnumerable<Order>> GetRecentAsync(int days = 90, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByContractSymbolAsync(string symbol, CancellationToken cancellationToken = default);
}

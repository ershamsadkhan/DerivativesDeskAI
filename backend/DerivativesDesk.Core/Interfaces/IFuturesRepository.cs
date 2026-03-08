using DerivativesDesk.Core.Models;

namespace DerivativesDesk.Core.Interfaces;

public interface IFuturesRepository
{
    Task<IEnumerable<FuturesContract>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FuturesContract>> GetExpiringWithinDaysAsync(int days, CancellationToken cancellationToken = default);
    Task<FuturesContract?> GetBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
}

using DerivativesDesk.Core.Models;

namespace DerivativesDesk.Core.Interfaces;

public interface IRolloverRepository
{
    Task<IEnumerable<RolloverPeriod>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RolloverPeriod>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RolloverPeriod>> GetUpcomingAsync(int daysAhead = 30, CancellationToken cancellationToken = default);
}

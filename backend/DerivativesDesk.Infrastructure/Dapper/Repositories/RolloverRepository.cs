using Dapper;
using DerivativesDesk.Core.Interfaces;
using DerivativesDesk.Core.Models;

namespace DerivativesDesk.Infrastructure.Dapper.Repositories;

public class RolloverRepository(ISqlConnectionFactory factory) : IRolloverRepository
{
    private const string BaseQuery = @"
        SELECT r.*,
               fc.Symbol AS FromSymbol,
               tc.Symbol AS ToSymbol
        FROM RolloverPeriods r
        JOIN FuturesContracts fc ON r.FromContractId = fc.ContractId
        JOIN FuturesContracts tc ON r.ToContractId   = tc.ContractId";

    public async Task<IEnumerable<RolloverPeriod>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var conn = factory.Create();
        return await conn.QueryAsync<RolloverPeriod>($"{BaseQuery} ORDER BY r.RollDate");
    }

    public async Task<IEnumerable<RolloverPeriod>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        using var conn = factory.Create();
        return await conn.QueryAsync<RolloverPeriod>(
            $@"{BaseQuery}
               WHERE r.RollStartDate <= CAST(GETUTCDATE() AS DATE)
                 AND r.RollDate      >= CAST(GETUTCDATE() AS DATE)
               ORDER BY r.RollDate");
    }

    public async Task<IEnumerable<RolloverPeriod>> GetUpcomingAsync(int daysAhead = 30, CancellationToken cancellationToken = default)
    {
        using var conn = factory.Create();
        return await conn.QueryAsync<RolloverPeriod>(
            $@"{BaseQuery}
               WHERE r.RollDate BETWEEN CAST(GETUTCDATE() AS DATE)
                                    AND CAST(DATEADD(DAY, @daysAhead, GETUTCDATE()) AS DATE)
               ORDER BY r.RollDate",
            new { daysAhead });
    }
}

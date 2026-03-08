using Dapper;
using DerivativesDesk.Core.Interfaces;
using DerivativesDesk.Core.Models;

namespace DerivativesDesk.Infrastructure.Dapper.Repositories;

public class FuturesRepository(ISqlConnectionFactory factory) : IFuturesRepository
{
    public async Task<IEnumerable<FuturesContract>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        using var conn = factory.Create();
        return await conn.QueryAsync<FuturesContract>(
            "SELECT * FROM FuturesContracts WHERE Status = 'Active' ORDER BY ExpiryDate");
    }

    public async Task<IEnumerable<FuturesContract>> GetExpiringWithinDaysAsync(int days, CancellationToken cancellationToken = default)
    {
        using var conn = factory.Create();
        return await conn.QueryAsync<FuturesContract>(
            @"SELECT * FROM FuturesContracts
              WHERE Status = 'Active'
                AND ExpiryDate BETWEEN CAST(GETUTCDATE() AS DATE) AND CAST(DATEADD(DAY, @days, GETUTCDATE()) AS DATE)
              ORDER BY ExpiryDate",
            new { days });
    }

    public async Task<FuturesContract?> GetBySymbolAsync(string symbol, CancellationToken cancellationToken = default)
    {
        using var conn = factory.Create();
        return await conn.QuerySingleOrDefaultAsync<FuturesContract>(
            "SELECT * FROM FuturesContracts WHERE Symbol = @symbol",
            new { symbol });
    }
}

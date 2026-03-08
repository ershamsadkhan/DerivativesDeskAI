using Dapper;
using DerivativesDesk.Core.Interfaces;
using DerivativesDesk.Core.Models;

namespace DerivativesDesk.Infrastructure.Dapper.Repositories;

public class OrdersRepository(ISqlConnectionFactory factory) : IOrdersRepository
{
    private const string BaseQuery = @"
        SELECT o.*,
               fc.Symbol  AS Symbol,
               pm.Name    AS PmName,
               pm.Type    AS PmType
        FROM Orders o
        JOIN FuturesContracts fc ON o.ContractId = fc.ContractId
        JOIN PortfolioManagers pm ON o.PmId      = pm.PmId";

    public async Task<IEnumerable<Order>> GetRecentAsync(int days = 90, CancellationToken cancellationToken = default)
    {
        using var conn = factory.Create();
        return await conn.QueryAsync<Order>(
            $@"{BaseQuery}
               WHERE o.OrderDate >= DATEADD(DAY, -@days, GETUTCDATE())
               ORDER BY o.OrderDate DESC",
            new { days });
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        using var conn = factory.Create();
        return await conn.QueryAsync<Order>(
            $@"{BaseQuery}
               WHERE o.Status = @status
               ORDER BY o.OrderDate DESC",
            new { status });
    }

    public async Task<IEnumerable<Order>> GetByContractSymbolAsync(string symbol, CancellationToken cancellationToken = default)
    {
        using var conn = factory.Create();
        return await conn.QueryAsync<Order>(
            $@"{BaseQuery}
               WHERE fc.Symbol = @symbol
               ORDER BY o.OrderDate DESC",
            new { symbol });
    }
}

using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DerivativesDesk.Infrastructure.Dapper;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}

public class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
{
    public IDbConnection Create() =>
        new SqlConnection(configuration.GetConnectionString("DerivativesDesk"));
}

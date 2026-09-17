using System.Data.Common;
using Microsoft.Data.SqlClient;
using SomnosSuite.Application;

namespace SomnosSuite.Persistence.Connections;

internal sealed class SqlServerConnectionFactory(
    string connectionString)
    : ISqlConnectionFactory
{
    public async Task<DbConnection> OpenConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(connectionString);

        await connection.OpenAsync(cancellationToken);

        return connection;
    }
}

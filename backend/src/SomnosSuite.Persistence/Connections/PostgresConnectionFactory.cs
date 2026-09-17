using System.Data.Common;
using Npgsql;
using SomnosSuite.Application;

namespace SomnosSuite.Persistence.Connections;

internal sealed class PostgresConnectionFactory(
    NpgsqlDataSource dataSource)
    : ISqlConnectionFactory
{
    public async Task<DbConnection> OpenConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        return await dataSource.OpenConnectionAsync(cancellationToken);
    }
}
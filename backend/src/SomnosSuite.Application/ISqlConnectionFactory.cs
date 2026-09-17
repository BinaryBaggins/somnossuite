using System.Data.Common;
namespace SomnosSuite.Application
{
    public interface ISqlConnectionFactory
    {
        Task<DbConnection> OpenConnectionAsync(
        CancellationToken cancellationToken = default);
    }
}
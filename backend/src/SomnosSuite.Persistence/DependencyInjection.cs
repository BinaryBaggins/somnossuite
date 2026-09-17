using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SomnosSuite.Application;
using SomnosSuite.Application.StunningDevices;
using SomnosSuite.Persistence.Connections;
using SomnosSuite.Persistence.StunningDevices;

namespace SomnosSuite.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "Connection string 'Database' is not configured.");

        services.AddSingleton(
            NpgsqlDataSource.Create(connectionString));

        services.AddSingleton<
            ISqlConnectionFactory,
            PostgresConnectionFactory>();

        services.AddScoped<
            IStunningDeviceRepository,
            StunningDeviceRepository>();

        return services;
    }
}
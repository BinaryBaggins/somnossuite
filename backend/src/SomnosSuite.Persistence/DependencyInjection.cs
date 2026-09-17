using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        services.AddSingleton<ISqlConnectionFactory>(
            new SqlServerConnectionFactory(connectionString));

        services.AddScoped<
            IStunningDeviceRepository,
            StunningDeviceRepository>();

        return services;
    }
}
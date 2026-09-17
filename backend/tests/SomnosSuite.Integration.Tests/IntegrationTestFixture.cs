using System.Data.Common;
using System.Reflection;
using DbUp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

namespace SomnosSuite.Integration.Tests;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private const string DatabaseName =
        "SomnosSuite_IntegrationTests";

    private readonly MsSqlContainer _sqlServer =
        new MsSqlBuilder(
            "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
        .Build();

    private WebApplicationFactory<Program>? _factory;

    public HttpClient Client =>
        _factory?.CreateClient()
        ?? throw new InvalidOperationException(
            "Integration test fixture has not been initialized.");

    public string ConnectionString { get; private set; } =
        string.Empty;

    public async Task InitializeAsync()
    {
        await _sqlServer.StartAsync();

        await _sqlServer.ExecScriptAsync(
            $"CREATE DATABASE [{DatabaseName}];");

        var connectionStringBuilder =
            new DbConnectionStringBuilder
            {
                ConnectionString =
                    _sqlServer.GetConnectionString()
            };

        connectionStringBuilder["Database"] = DatabaseName;

        ConnectionString =
            connectionStringBuilder.ConnectionString;

        var upgrader = DeployChanges.To
            .SqlDatabase(ConnectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var migrationResult = upgrader.PerformUpgrade();

        if (!migrationResult.Successful)
        {
            throw new InvalidOperationException(
                "Integration test database migration failed.",
                migrationResult.Error);
        }

        _factory =
            new IntegrationTestWebApplicationFactory(
                ConnectionString);
    }

    public async Task DisposeAsync()
    {
        _factory?.Dispose();

        await _sqlServer.DisposeAsync();
    }

    private sealed class IntegrationTestWebApplicationFactory(
    string connectionString)
    : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(configuration =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Database"] =
                            connectionString
                    });
            });

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }
    }
}
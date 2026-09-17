using System.Reflection;
using DbUp;

var connectionString =
    Environment.GetEnvironmentVariable(
        "SOMNOSSUITE_DB_CONNECTION");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine(
        "Environment variable SOMNOSSUITE_DB_CONNECTION is not set.");

    return 1;
}

var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(
        Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.Error.WriteLine(result.Error);
    return 1;
}

Console.WriteLine(
    "Database migration completed successfully.");

return 0;
# Development Setup

This guide describes how to run SomnosSuite locally.

## Prerequisites

Install the following tools:

- Git
- .NET SDK 10
- Node.js 22.13.0 or newer
- npm 10 or newer
- Docker Desktop
- A SQL Server client such as SQL Server Management Studio or the VS Code MSSQL extension

The repository pins the .NET SDK through `global.json` and the Node.js version through `.nvmrc`.

Verify the installations:

```powershell
dotnet --version
node --version
npm --version
docker --version
docker compose version
```

## 1. Clone the Repository

```powershell
git clone https://github.com/BinaryBaggins/somnossuite.git
cd somnossuite
```

## 2. Configure SQL Server

Local development uses SQL Server 2022 Developer running in Docker.

Create a `.env` file in the repository root:

```env
MSSQL_SA_PASSWORD=YourStrongLocalPassword
```

Do not commit this file.

Start SQL Server:

```powershell
docker compose up -d
```

Check that the container is running:

```powershell
docker compose ps
```

If SQL Server is still starting, inspect its logs:

```powershell
docker compose logs -f sqlserver
```

The local SQL Server is exposed on:

```text
Server: localhost,1433
User:   sa
```

Use the password configured in `.env`.

## 3. Create the Development Database

Connect to the local SQL Server with your preferred SQL client and run:

```sql
IF DB_ID(N'SomnosSuite_Dev') IS NULL
BEGIN
    CREATE DATABASE [SomnosSuite_Dev];
END;
```

The database only needs to be created once.

Schema changes are applied separately through the database migrator.

## 4. Configure the Backend Connection String

The Web API uses .NET User Secrets for the local database connection.

From the repository root:

```powershell
dotnet user-secrets set `
  "ConnectionStrings:Database" `
  "Server=localhost,1433;Database=SomnosSuite_Dev;User Id=sa;Password=YourStrongLocalPassword;Encrypt=True;TrustServerCertificate=True;" `
  --project backend/src/SomnosSuite.WebApi/SomnosSuite.WebApi.csproj
```

Verify the configured secrets:

```powershell
dotnet user-secrets list `
  --project backend/src/SomnosSuite.WebApi/SomnosSuite.WebApi.csproj
```

Do not store development passwords in `appsettings.json`.

## 5. Apply Database Migrations

The database migrator reads its connection string from the `SOMNOSSUITE_DB_CONNECTION` environment variable.

In PowerShell:

```powershell
$env:SOMNOSSUITE_DB_CONNECTION = `
  "Server=localhost,1433;Database=SomnosSuite_Dev;User Id=sa;Password=YourStrongLocalPassword;Encrypt=True;TrustServerCertificate=True;"
```

Run the migrations:

```powershell
dotnet run `
  --project backend/tools/SomnosSuite.DatabaseMigrator/SomnosSuite.DatabaseMigrator.csproj
```

A successful run ends with:

```text
Database migration completed successfully.
```

The migrator is idempotent. Running it again only applies migrations that have not already been executed.

## 6. Build and Test the Backend

Restore dependencies:

```powershell
dotnet restore backend/backend.sln
```

Build the solution:

```powershell
dotnet build backend/backend.sln --no-restore
```

Run all backend tests:

```powershell
dotnet test backend/backend.sln --no-build
```

The integration tests start their own isolated SQL Server container through Testcontainers.

The local development database is not used by the integration tests.

## 7. Run the Web API

If the ASP.NET Core development certificate has not been trusted yet:

```powershell
dotnet dev-certs https --trust
```

Start the API:

```powershell
dotnet run `
  --project backend/src/SomnosSuite.WebApi/SomnosSuite.WebApi.csproj `
  --launch-profile https
```

The API is available at:

```text
https://localhost:7086
http://localhost:5063
```

## 8. Configure the Frontend

Move to the frontend directory:

```powershell
cd frontend
```

Create the local environment file:

```powershell
Copy-Item .env.example .env
```

The default configuration points to the local HTTPS API:

```env
PUBLIC_API_BASE_URL="https://localhost:7086"
```

Install dependencies:

```powershell
npm ci
```

Run the frontend:

```powershell
npm run dev
```

## 9. Frontend Validation

Run the Svelte and TypeScript checks:

```powershell
npm run check
```

Run linting:

```powershell
npm run lint
```

Build the production frontend:

```powershell
npm run build
```

## Daily Development Workflow

After the initial setup, the normal workflow is much shorter.

Start SQL Server:

```powershell
docker compose up -d
```

Apply new database migrations when necessary:

```powershell
$env:SOMNOSSUITE_DB_CONNECTION = `
  "Server=localhost,1433;Database=SomnosSuite_Dev;User Id=sa;Password=YourStrongLocalPassword;Encrypt=True;TrustServerCertificate=True;"

dotnet run `
  --project backend/tools/SomnosSuite.DatabaseMigrator/SomnosSuite.DatabaseMigrator.csproj
```

Start the backend:

```powershell
dotnet run `
  --project backend/src/SomnosSuite.WebApi/SomnosSuite.WebApi.csproj `
  --launch-profile https
```

Start the frontend in another terminal:

```powershell
cd frontend
npm run dev
```

## Stopping the Development Environment

Stop SQL Server:

```powershell
docker compose down
```

The database remains stored in the Docker volume and will be available the next time the container starts.

To completely remove the local SQL Server data:

```powershell
docker compose down -v
```

> Warning: `docker compose down -v` permanently deletes the local SQL Server volume and all databases stored in it.

## Troubleshooting

### SQL Server is not reachable

Check whether the container is running:

```powershell
docker compose ps
```

Inspect the SQL Server logs:

```powershell
docker compose logs sqlserver
```

Also verify that port `1433` is not already used by another local SQL Server instance.

### The migrator cannot find its connection string

Make sure the environment variable exists in the current PowerShell session:

```powershell
$env:SOMNOSSUITE_DB_CONNECTION
```

Environment variables assigned this way only exist for the current PowerShell session.

### HTTPS certificate errors

Trust the ASP.NET Core development certificate:

```powershell
dotnet dev-certs https --trust
```

Then restart the Web API and browser.

### Reset the local database completely

Remove the Docker volume:

```powershell
docker compose down -v
docker compose up -d
```

Create `SomnosSuite_Dev` again and rerun the database migrator.

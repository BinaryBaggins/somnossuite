# SomnosSuite

SomnosSuite is a web application for livestock stunning-control and documentation workflows.

The repository contains a .NET backend, a SvelteKit frontend, SQL Server persistence, database migrations, and automated tests.

## Tech Stack

- .NET 10 / ASP.NET Core
- MediatR
- Dapper
- SQL Server 2022
- DbUp
- SvelteKit
- TypeScript
- Docker
- xUnit / Testcontainers
- GitHub Actions / CodeQL

## Repository Structure

```text
.
├── backend/
│   ├── database/
│   ├── src/
│   ├── tests/
│   ├── tools/
│   └── backend.sln
├── docs/
├── frontend/
├── compose.yml
└── global.json
```

## Development

For the complete local development setup, see:

[Development Setup](docs/DEVELOPMENT_SETUP.md)

Once the environment is configured, start SQL Server:

```powershell
docker compose up -d
```

Run the backend tests:

```powershell
dotnet test backend/backend.sln
```

Start the Web API:

```powershell
dotnet run `
  --project backend/src/SomnosSuite.WebApi/SomnosSuite.WebApi.csproj `
  --launch-profile https
```

Start the frontend in a separate terminal:

```powershell
cd frontend
npm run dev
```

The local API is available at:

- `https://localhost:7086`
- `http://localhost:5063`

## Documentation

- [Development Setup](docs/DEVELOPMENT_SETUP.md)
- [Domain Plan](docs/DOMAIN_PLAN.md)
- [Domain Rules](docs/DOMAIN_RULES.md)
- [Domain Roadmap](docs/DOMAIN_ROADMAP.md)

## License

See [LICENSE](LICENSE).

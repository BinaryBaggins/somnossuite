# Contributing

Thanks for helping improve SomnosSuite.

## Setup

Install the required toolchain:

- .NET SDK `10.0.300`
- Node.js `22.13.0` or newer
- npm `10` or newer

Backend:

```powershell
cd backend
dotnet restore backend.sln
dotnet build backend.sln
dotnet test backend.sln
```

Frontend:

```powershell
cd frontend
npm ci
npm run check
npm run lint
npm run build
```

## Environment

Copy `frontend\.env.example` to `frontend\.env` for local development. Never commit `.env` files, credentials, production data, or private machine paths.

## Documentation

Domain documentation lives in `docs/`. Keep `DOMAIN_MODEL.md`, `DOMAIN_RULES.md`, and `DOMAIN_ROADMAP.md` aligned with code and tests when domain behavior changes.

## Pull Requests

Before opening a pull request:

- keep changes focused
- update tests or docs when behavior changes
- run the relevant backend and frontend checks
- avoid committing generated output such as `bin/`, `obj/`, `.svelte-kit/`, or build artifacts

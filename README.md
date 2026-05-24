# Dapper + PostgreSQL Learning Project

This project contains:
- `.NET 10` Web API (`DapperSqlLearning.Api`)
- `Dapper` for SQL queries
- `PostgreSQL` database
- `pgAdmin` for database UI
- `Docker Compose` to run everything together

## Run everything together (Podman)

Use the helper script:

```bash
./start.sh
```

Windows / Podman Desktop (recommended):

```powershell
.\start.ps1
```

Rebuild images when needed:

```bash
./start.sh --rebuild
```

Windows / Podman Desktop rebuild:

```powershell
.\start.ps1 -Rebuild
```

Or run manually:

```powershell
podman compose up --build -d
```

Then open:
- API root: `http://localhost:8080/`
- OpenAPI JSON: `http://localhost:8080/openapi/v1.json`
- pgAdmin: `http://localhost:5050`

pgAdmin login:
- Email: `admin@local.dev`
- Password: `admin123`

Database connection in pgAdmin:
- Host: `host.containers.internal`
- Port: `5432`
- DB: `dapper_learning`
- User: `postgres`
- Password: `postgrespw`

## Stop stack

```powershell
podman compose down
```

If you also want to reset data:

```powershell
podman compose down -v
```

## API endpoints for Dapper practice

- `GET /products`
- `GET /products/{id}`
- `POST /products`
- `PUT /products/{id}`
- `DELETE /products/{id}`

`POST /products` body example:

```json
{
  "name": "Monitor",
  "price": 199.99
}
```

Notes:
- `products.id` is now app-generated UUID text for cross-database portability.
- If you already had an older Postgres volume with integer identity `products.id`, reset volumes once:
  `podman compose down -v`

## Switch database driver (Postgres or SQLite)

Set provider in `DapperSqlLearning.Api/appsettings.json`:

```json
"Database": {
  "Provider": "postgres"
}
```

Supported values:
- `postgres`
- `sqlite`

Connection strings:
- `ConnectionStrings:Postgres`
- `ConnectionStrings:Sqlite`

SQLite file note:
- Relative SQLite paths are resolved from `AppContext.BaseDirectory` (runtime output, e.g. `bin/Debug/net10.0`).

Environment override examples:

```powershell
$env:Database__Provider='sqlite'
$env:ConnectionStrings__Sqlite='Data Source=sqlite-check.db'
dotnet run --no-launch-profile --project .\DapperSqlLearning.Api\DapperSqlLearning.Api.csproj
```

```powershell
$env:Database__Provider='postgres'
$env:ConnectionStrings__Postgres='Host=localhost;Port=5432;Database=dapper_learning;Username=postgres;Password=postgrespw'
dotnet run --no-launch-profile --project .\DapperSqlLearning.Api\DapperSqlLearning.Api.csproj
```

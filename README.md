# 🚀 Master Dapper with Multiple SQL Providers

Welcome to the **Dapper SQL Learning Project**! This public repository is built for anyone who wants to learn and practice using [Dapper](https://github.com/DapperLib/Dapper) across different database engines.

The project connects a modern `.NET 10 Web API` with **three** different SQL providers. It also includes out-of-the-box UI tools to manage your databases effortlessly!

### 🌟 What's Inside?
- **.NET 10 Web API** (`DapperSqlLearning.Api`) pre-configured with Dapper.
- **Multiple Supported Databases:** PostgreSQL, MySQL, and SQLite.
- **Database UI Tools:** `pgAdmin` for PostgreSQL and `phpMyAdmin` for MySQL.
- **Docker Compose** environment to spin everything up with zero configuration.

---

## 🔑 Database Credentials & Access

No need to dig through Dockerfiles! Here is everything you need to connect to the databases:

### 🐘 PostgreSQL (via pgAdmin)
- **UI Access:** [http://localhost:5050](http://localhost:5050)
- **pgAdmin Login:** Email: `admin@local.dev` | Password: `root`
- **Database Connection Info:**
  - Host: `postgres` (or `localhost` outside docker)
  - Port: `5432`
  - Database: `dapper_learning`
  - User: `postgres`
  - Password: `postgrespw`

### 🐬 MySQL (via phpMyAdmin)
- **UI Access:** [http://localhost:8081](http://localhost:8081)
- **phpMyAdmin Login:** User: `root` | Password: `rootpassword`
- **Database Connection Info:**
  - Host: `mysql` (or `localhost` outside docker)
  - Port: `3306`
  - Database: `dapper_learning`
  - User: `mysqluser`
  - Password: `mysqlpassword`
  - Root Password: `rootpassword`

### 🪶 SQLite
- **Connection String:** `Data Source=dapper_learning.db`
- Automatically generated locally by the application.

---

## 🚀 Getting Started

The easiest way to spin up the entire stack is by using the provided shell scripts.

### Run everything together (Docker / Podman)

**Mac / Linux / Windows (via Git Bash):**
```bash
./start.sh
```

Once running, the API is available at:
- **API Root:** [http://localhost:8080/](http://localhost:8080/)
- **OpenAPI / Swagger:** [http://localhost:8080/openapi/v1.json](http://localhost:8080/openapi/v1.json)

*(Need to rebuild images? Just pass `--rebuild` to the script)*

### Stop the stack

```bash
docker compose down
```
*(To completely reset your databases, add the `-v` flag: `docker compose down -v`)*

---

## ⚙️ Switching the Database Provider

Want to practice Dapper on a different database? It's as easy as flipping a switch!

Set the active provider in `DapperSqlLearning.Api/appsettings.json`:

```json
"Database": {
  "Provider": "mysql" // Supports: "postgres", "mysql", "sqlite"
}
```

The application will automatically use the correct connection string matching the provider!

---

## 📚 API Endpoints for Dapper Practice

The API ships with a `ProductRepository` that demonstrates CRUD operations using Dapper.

- `GET /products` - Fetch all products
- `GET /products/{id}` - Fetch a specific product
- `POST /products` - Create a new product
- `PUT /products/{id}` - Update a product
- `DELETE /products/{id}` - Delete a product

**Example POST body:**
```json
{
  "name": "Monitor",
  "price": 199.99
}
```

*Happy learning, and may your queries be fast!* 🚀

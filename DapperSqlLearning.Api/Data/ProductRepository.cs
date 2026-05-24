using Dapper;
using DapperSqlLearning.Api.Dtos;
using DapperSqlLearning.Api.Models;

namespace DapperSqlLearning.Api.Data;

public sealed class ProductRepository(IAppDbConnectionFactory connectionFactory) : IProductRepository
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS products (
                id VARCHAR(36) PRIMARY KEY,
                name TEXT NOT NULL,
                price DECIMAL(10, 2) NOT NULL CHECK (price > 0),
                created_at TIMESTAMP NOT NULL
            );

            INSERT INTO products (id, name, price, created_at)
            SELECT '8d0fd3af-2e09-4eef-9ec0-2f4eeb91a490', 'Notebook', 3.49, CURRENT_TIMESTAMP
            FROM (SELECT 1) AS dummy
            WHERE NOT EXISTS (SELECT 1 FROM products WHERE id = '8d0fd3af-2e09-4eef-9ec0-2f4eeb91a490');

            INSERT INTO products (id, name, price, created_at)
            SELECT '9ecce17a-44ed-40c4-bbb9-bf0675f00558', 'Keyboard', 49.99, CURRENT_TIMESTAMP
            FROM (SELECT 1) AS dummy
            WHERE NOT EXISTS (SELECT 1 FROM products WHERE id = '9ecce17a-44ed-40c4-bbb9-bf0675f00558');
            """;

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                id AS Id,
                name AS Name,
                price AS Price,
                created_at AS CreatedAt
            FROM products
            ORDER BY id;
            """;

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var result = await connection.QueryAsync<Product>(command);
        return result.AsList();
    }

    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                id AS Id,
                name AS Name,
                price AS Price,
                created_at AS CreatedAt
            FROM products
            WHERE id = @Id;
            """;

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Product>(command);
    }

    public async Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var createdId = Guid.NewGuid().ToString("D");
        var createdAt = DateTime.UtcNow;
        var parameters = new
        {
            Id = createdId,
            Name = request.Name.Trim(),
            request.Price,
            CreatedAt = createdAt
        };
        
        const string insertSql = """
            INSERT INTO products (id, name, price, created_at)
            VALUES (@Id, @Name, @Price, @CreatedAt);
            """;

        const string readInsertedSql = """
            SELECT
                id AS Id,
                name AS Name,
                price AS Price,
                created_at AS CreatedAt
            FROM products
            WHERE id = @Id;
            """;

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(new CommandDefinition(insertSql, parameters, cancellationToken: cancellationToken));
        var created = await connection.QuerySingleAsync<Product>(new CommandDefinition(readInsertedSql, new { Id = createdId }, cancellationToken: cancellationToken));
        return created;
    }

    public async Task<bool> UpdateAsync(string id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE products
            SET name = @Name, price = @Price
            WHERE id = @Id;
            """;

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var parameters = new
        {
            Id = id,
            Name = request.Name.Trim(),
            request.Price
        };

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        var affectedRows = await connection.ExecuteAsync(command);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM products WHERE id = @Id;";

        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        var affectedRows = await connection.ExecuteAsync(command);
        return affectedRows > 0;
    }

    private System.Data.Common.DbConnection CreateConnection() => connectionFactory.CreateConnection();
}

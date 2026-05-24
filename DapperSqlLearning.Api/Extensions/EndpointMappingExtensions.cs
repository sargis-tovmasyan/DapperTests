using DapperSqlLearning.Api.Data;
using DapperSqlLearning.Api.Dtos;

namespace DapperSqlLearning.Api.Extensions;

public static class EndpointMappingExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new
        {
            message = "Dapper SQL learning API is running.",
            openApi = "/openapi/v1.json"
        }));

        app.MapGet("/health", () => Results.Ok(new
        {
            status = "ok",
            utc = DateTime.UtcNow
        }));

        app.MapGet("/products", async (IProductRepository repository, CancellationToken cancellationToken) =>
        {
            var products = await repository.GetAllAsync(cancellationToken);
            return Results.Ok(products);
        });

        app.MapGet("/products/{id}", async (string id, IProductRepository repository, CancellationToken cancellationToken) =>
        {
            var product = await repository.GetByIdAsync(id, cancellationToken);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        app.MapPost("/products", async (CreateProductRequest request, IProductRepository repository, CancellationToken cancellationToken) =>
        {
            var validation = ValidateProductInput(request.Name, request.Price);
            if (validation is not null)
            {
                return validation;
            }

            var created = await repository.CreateAsync(request, cancellationToken);
            return Results.Created($"/products/{created.Id}", created);
        });

        app.MapPut("/products/{id}", async (string id, UpdateProductRequest request, IProductRepository repository, CancellationToken cancellationToken) =>
        {
            var validation = ValidateProductInput(request.Name, request.Price);
            if (validation is not null)
            {
                return validation;
            }

            var updated = await repository.UpdateAsync(id, request, cancellationToken);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        app.MapDelete("/products/{id}", async (string id, IProductRepository repository, CancellationToken cancellationToken) =>
        {
            var deleted = await repository.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }

    private static IResult? ValidateProductInput(string name, decimal price)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors["name"] = ["Name is required."];
        }

        if (price <= 0)
        {
            errors["price"] = ["Price must be greater than 0."];
        }

        return errors.Count == 0 ? null : Results.ValidationProblem(errors);
    }
}

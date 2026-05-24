using DapperSqlLearning.Api.Dtos;
using DapperSqlLearning.Api.Models;

namespace DapperSqlLearning.Api.Data;

public interface IProductRepository
{
    Task InitializeAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(string id, UpdateProductRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}

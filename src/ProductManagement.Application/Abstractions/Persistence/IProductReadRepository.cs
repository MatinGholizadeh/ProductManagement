using ProdManagement.Domain.Entities.Products;

namespace ProdManagement.Application.Abstractions.Persistence;

public interface IProductReadRepository
{
    Task<List<Product>> GetAllAsync();
    Task<bool> ExistsByEmailAndDateAsync(string manufactureEmail, DateTime produceDate, CancellationToken cancellationToken);
    Task<List<Product>> GetByCreatedByAsync(string createdBy, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

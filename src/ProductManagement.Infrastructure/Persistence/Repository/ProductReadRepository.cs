using Microsoft.EntityFrameworkCore;
using ProdManagement.Domain.Entities.Products;
using ProdManagement.Application.Abstractions.Persistence;

namespace ProdManagement.Infrastructure.Persistence.Repository;

public class ProductReadRepository : IProductReadRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductReadRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _dbContext.Products.ToListAsync();
    }

    public async Task<List<Product>> GetByCreatorAsync(string createdBy)
    {
        return await _dbContext.Products
            .Where(p => p.CreatedBy == createdBy)
            .ToListAsync();
    }

    public async Task<bool> ExistsByEmailAndDateAsync(string manufactureEmail, DateTime produceDate, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AnyAsync(p => p.ManufactureEmail == manufactureEmail && p.ProductDate.Date == produceDate.Date, cancellationToken);
    }

    public async Task<List<Product>> GetByCreatedByAsync(string createdBy, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Where(p => p.CreatedBy == createdBy)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
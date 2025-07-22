using Microsoft.EntityFrameworkCore;
using ProdManagement.Domain.Entities.Products;

namespace ProdManagement.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

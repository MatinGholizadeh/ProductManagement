using Microsoft.EntityFrameworkCore;
using ProdManagement.Domain.Entities.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProdManagement.Application.Abstractions.Persistence;
using ProdManagement.Domain.Entities.Products;

namespace ProdManagement.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    #region Constructor

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

    #endregion Constructor - End

    #region DbSet

    public DbSet<Product> Products { get; set; } = default!;

    #endregion DbSet - End

    #region SaveChangesAsync

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    #endregion SaveChangesAsync - End

    #region OnModelCreating

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Let it be on top, always!!!
        base.OnModelCreating(builder);

        // Config unique ManufactureEmail & ProductDate - Fluent API
        builder.Entity<Product>()
            .HasIndex(p => new { p.ManufactureEmail, p.ProductDate })
            .IsUnique();
    }

    #endregion OnModelCreating - End

}

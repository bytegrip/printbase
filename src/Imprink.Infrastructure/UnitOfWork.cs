using Imprink.Application;
using Imprink.Domain.Repositories;
using Imprink.Infrastructure.Database;

namespace Imprink.Infrastructure;

public class UnitOfWork(
    ApplicationDbContext context, 
    IProductRepository productRepository, 
    IProductVariantRepository productVariantRepository, 
    ICategoryRepository categoryRepository) : IUnitOfWork
{
    public IProductRepository ProductRepository => productRepository;
    public IProductVariantRepository ProductVariantRepository => productVariantRepository;
    public ICategoryRepository CategoryRepository => categoryRepository;

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await context.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await context.Database.RollbackTransactionAsync(cancellationToken);
    }
}
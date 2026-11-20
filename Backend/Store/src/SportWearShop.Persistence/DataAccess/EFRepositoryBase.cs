using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportWearShop.Application.Abstractions.DataAccess;

namespace SportWearShop.Persistence.DataAccess;

public class EFRepositoryBase<TEntity, TKey>(StoreDbContext context)
    : IRepository<TEntity, TKey>
    where TEntity : class
    where TKey : notnull
{
    protected DbSet<TEntity> Entities { get; } = context.Set<TEntity>();

    public ValueTask<TEntity?> FindAsync(
        TKey id, CancellationToken cancellationToken = default)
        => Entities.FindAsync([id], cancellationToken);

    public Task<bool> AnyAsync(
        ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification, evaluateCriteriaOnly: true)
            .AnyAsync(cancellationToken);

    public Task<int> CountAsync(
        ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification, evaluateCriteriaOnly: true)
            .CountAsync(cancellationToken);

    public Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(
        ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);

    public Task<List<TEntity>> ListAsync(
        ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).ToListAsync(cancellationToken);

    public Task<List<TResult>> ListAsync<TResult>(
        ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default)
        => ApplySpecification(specification).ToListAsync(cancellationToken);

    public void Add(TEntity entity) => Entities.Add(entity);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Entities.AddAsync(entity, cancellationToken);
    }

    public void Remove(TEntity entity) => Entities.Remove(entity);

    public void UpdateDetached(TEntity entity) => Entities.Update(entity);

    protected virtual IQueryable<TEntity> ApplySpecification(
        ISpecification<TEntity> specification, bool evaluateCriteriaOnly = false)
        => SpecificationEvaluator.Default.GetQuery(Entities, specification, evaluateCriteriaOnly);

    protected virtual IQueryable<TResult> ApplySpecification<TResult>(
        ISpecification<TEntity, TResult> specification)
        => SpecificationEvaluator.Default.GetQuery(Entities, specification);
}

using Ardalis.Specification;

namespace SportWearShop.Application.Abstractions.DataAccess;

public interface IRepository<TEntity, in TKey>
    where TEntity : class
    where TKey : notnull
{
    ValueTask<TEntity?> FindAsync(
        TKey id, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task<TResult?> FirstOrDefaultAsync<TResult>(
        ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);

    Task<List<TEntity>> ListAsync(
        ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task<List<TResult>> ListAsync<TResult>(
        ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);

    void Add(TEntity entity);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Remove(TEntity entity);
    void UpdateDetached(TEntity entity);
}

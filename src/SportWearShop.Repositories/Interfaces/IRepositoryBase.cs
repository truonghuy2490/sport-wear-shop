using System.Linq.Expressions;

namespace SportWearShop.Repositories.Interfaces;

public interface IBaseRepository<T> where T : class
{

    Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task<List<TResult>> FindAsync<TResult>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, object>>? sortBy = null,
        bool ascending = true,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default);

    void Update(T entity);

    void UpdateRange(IEnumerable<T> entities);

    void Remove(T entity);

    void RemoveRange(IEnumerable<T> entities);

    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}
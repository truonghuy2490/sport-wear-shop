using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SportWearShop.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SportWearShop.Repositories.Implementations;

/// <summary>
/// Repository base class with optimized performance, filtering, paging, and projection support
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
/// 
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<BaseRepository<T>> _logger;

    public BaseRepository(
        AppDbContext context,
        ILogger<BaseRepository<T>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public virtual IQueryable<T> Query(bool asNoTracking = true)
    {
        IQueryable<T> query = _dbSet;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
    /*
    /// <summary>
    /// Get queryable with optional tracking and includes
    /// </summary>
    protected IQueryable<T> Query(
        bool asNoTracking = true,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        foreach (var include in includes)
            query = query.Include(include);

        return query;
    }*/

    public virtual async Task<T?> GetByIdAsync(
        object id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Log: "Getting {EntityName} by id {Id}"
            _logger.LogInformation(
                "Getting {EntityName} by id {Id}",
                typeof(T).Name,
                id);

            return await _dbSet.FindAsync(new[] { id }, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: "Error while getting {EntityName} by id {Id}"
            _logger.LogError(
                ex,
                "Error while getting {EntityName} by id {Id}",
                typeof(T).Name,
                id);

            throw;
        }
    }

    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Log: "Getting first {EntityName} by predicate. Tracking: {Tracking}"
            _logger.LogInformation(
                "Getting first {EntityName} by predicate. Tracking: {Tracking}",
                typeof(T).Name,
                !asNoTracking);

            return await Query(asNoTracking)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: "Error while getting first {EntityName} by predicate"
            _logger.LogError(
                ex,
                "Error while getting first {EntityName} by predicate",
                typeof(T).Name);

            throw;
        }
    }

    public async Task<(List<TResult> Items, int TotalCount)> GetAllAsync<TResult>(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, TResult>>? selector = null,
        int pageNumber = 1,
        int pageSize = 10,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        int totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
            query = orderBy(query);

        if (pageNumber > 0 && pageSize > 0)
        {
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        if (selector == null)
            throw new ArgumentNullException(nameof(selector));

        var items = await query
            .Select(selector)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Log : "Finding {EntityName} by predicate. Tracking: {Tracking}"
            _logger.LogInformation(
                "Finding {EntityName} by predicate. Tracking: {Tracking}",
                typeof(T).Name,
                !asNoTracking);

            return await Query(asNoTracking)
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: "Error while finding {EntityName} by predicate"
            _logger.LogError(
                ex,
                "Error while finding {EntityName}",
                typeof(T).Name);

            throw;
        }
    }

    public virtual async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            // Log: "Adding new {EntityName}"
            _logger.LogInformation(
                "Adding new {EntityName}",
                typeof(T).Name);

            await _dbSet.AddAsync(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: "Error while adding {EntityName}"
            _logger.LogError(
                ex,
                "Error while adding {EntityName}",
                typeof(T).Name);

            throw;
        }
    }

    public virtual async Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            // Log: "Adding range of {EntityName}"
            _logger.LogInformation(
                "Adding range of {EntityName}",
                typeof(T).Name);

            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: "Error while adding range of {EntityName}"
            _logger.LogError(
                ex,
                "Error while adding range of {EntityName}",
                typeof(T).Name);

            throw;
        }
    }

    public virtual void Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            // Log: "Updating {EntityName}"
            _logger.LogInformation(
                "Updating {EntityName}",
                typeof(T).Name);

            _dbSet.Update(entity);
        }
        catch (Exception ex)
        {
            // Log: "Error while updating {EntityName}"
            _logger.LogError(
                ex,
                "Error while updating {EntityName}",
                typeof(T).Name);

            throw;
        }
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            // Log: "Updating range of {EntityName}"
            _logger.LogInformation(
                "Updating range of {EntityName}",
                typeof(T).Name);

            _dbSet.UpdateRange(entities);
        }
        catch (Exception ex)
        {
            // Log: "Error while updating range of {EntityName}"
            _logger.LogError(
                ex,
                "Error while updating range of {EntityName}",
                typeof(T).Name);

            throw;
        }
    }

    public virtual void Remove(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            // Log: "Removing {EntityName}"
            _logger.LogInformation(
                "Removing {EntityName}",
                typeof(T).Name);

            _dbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            // Log: "Error while removing {EntityName}"
            _logger.LogError(
                ex,
                "Error while removing {EntityName}",
                typeof(T).Name);

            throw;
        }
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            // Log: "Removing range of {EntityName}"
            _logger.LogInformation(
                "Removing range of {EntityName}",
                typeof(T).Name);

            _dbSet.RemoveRange(entities);
        }
        catch (Exception ex)
        {
            // Log: "Error while removing range of {EntityName}"
            _logger.LogError(
                ex,
                "Error while removing range of {EntityName}",
                typeof(T).Name);

            throw;
        }
    }

    public virtual async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Log: "Checking existence for {EntityName}"
            _logger.LogInformation(
                "Checking existence for {EntityName}",
                typeof(T).Name);

            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: "Error while checking existence for {EntityName}"
            _logger.LogError(
                ex,
                "Error while checking existence for {EntityName}",
                typeof(T).Name);

            throw;
        }
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Log: "Counting {EntityName}"
            _logger.LogInformation(
                "Counting {EntityName}",
                typeof(T).Name);

            return predicate == null
                ? await _dbSet.CountAsync(cancellationToken)
                : await _dbSet.CountAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: "Error while counting {EntityName}"
            _logger.LogError(
                ex,
                "Error while counting {EntityName}",
                typeof(T).Name);

            throw;
        }
    }

}

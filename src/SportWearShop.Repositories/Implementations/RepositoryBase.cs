using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SportWearShop.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SportWearShop.Repositories.Implementations;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly ILogger<BaseRepository<TEntity>> _logger;

    public BaseRepository(
        AppDbContext context,
        ILogger<BaseRepository<TEntity>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<TEntity>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual IQueryable<TEntity> Query(bool asNoTracking = true)
    {
        IQueryable<TEntity> query = _dbSet;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        object id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Log: "Getting {EntityName} by id {Id}"
            _logger.LogInformation(
                "Getting {EntityName} by id {Id}",
                typeof(TEntity).Name,
                id);

            return await _dbSet.FindAsync(new[] { id }, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: "Error while getting {EntityName} by id {Id}"
            _logger.LogError(
                ex,
                "Error while getting {EntityName} by id {Id}",
                typeof(TEntity).Name,
                id);

            throw;
        }
    }

    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Log: "Getting first {EntityName} by predicate. Tracking: {Tracking}"
            _logger.LogInformation(
                "Getting first {EntityName} by predicate. Tracking: {Tracking}",
                typeof(TEntity).Name,
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
                typeof(TEntity).Name);

            throw;
        }
    }

    public async Task<(List<TResult> Items, int TotalCount)> GetAllAsync<TResult>(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Expression<Func<TEntity, TResult>>? selector = null,
        int pageNumber = 1,
        int pageSize = 10,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

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

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Log : "Finding {EntityName} by predicate. Tracking: {Tracking}"
            _logger.LogInformation(
                "Finding {EntityName} by predicate. Tracking: {Tracking}",
                typeof(TEntity).Name,
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
                typeof(TEntity).Name);

            throw;
        }
    }

    public virtual async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            // Log: "Adding new {EntityName}"
            _logger.LogInformation(
                "Adding new {EntityName}",
                typeof(TEntity).Name);

            await _dbSet.AddAsync(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: "Error while adding {EntityName}"
            _logger.LogError(
                ex,
                "Error while adding {EntityName}",
                typeof(TEntity).Name);

            throw;
        }
    }

    public virtual async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            // Log: "Adding range of {EntityName}"
            _logger.LogInformation(
                "Adding range of {EntityName}",
                typeof(TEntity).Name);

            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: "Error while adding range of {EntityName}"
            _logger.LogError(
                ex,
                "Error while adding range of {EntityName}",
                typeof(TEntity).Name);

            throw;
        }
    }

    public virtual void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            // Log: "Updating {EntityName}"
            _logger.LogInformation(
                "Updating {EntityName}",
                typeof(TEntity).Name);

            _dbSet.Update(entity);
        }
        catch (Exception ex)
        {
            // Log: "Error while updating {EntityName}"
            _logger.LogError(
                ex,
                "Error while updating {EntityName}",
                typeof(TEntity).Name);

            throw;
        }
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            // Log: "Updating range of {EntityName}"
            _logger.LogInformation(
                "Updating range of {EntityName}",
                typeof(TEntity).Name);

            _dbSet.UpdateRange(entities);
        }
        catch (Exception ex)
        {
            // Log: "Error while updating range of {EntityName}"
            _logger.LogError(
                ex,
                "Error while updating range of {EntityName}",
                typeof(TEntity).Name);

            throw;
        }
    }

    public virtual void Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            // Log: "Removing {EntityName}"
            _logger.LogInformation(
                "Removing {EntityName}",
                typeof(TEntity).Name);

            _dbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            // Log: "Error while removing {EntityName}"
            _logger.LogError(
                ex,
                "Error while removing {EntityName}",
                typeof(TEntity).Name);

            throw;
        }
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        try
        {
            // Log: "Removing range of {EntityName}"
            _logger.LogInformation(
                "Removing range of {EntityName}",
                typeof(TEntity).Name);

            _dbSet.RemoveRange(entities);
        }
        catch (Exception ex)
        {
            // Log: "Error while removing range of {EntityName}"
            _logger.LogError(
                ex,
                "Error while removing range of {EntityName}",
                typeof(TEntity).Name);

            throw;
        }
    }

    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Log: "Checking existence for {EntityName}"
            _logger.LogInformation(
                "Checking existence for {EntityName}",
                typeof(TEntity).Name);

            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log: "Error while checking existence for {EntityName}"
            _logger.LogError(
                ex,
                "Error while checking existence for {EntityName}",
                typeof(TEntity).Name);

            throw;
        }
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Log: "Counting {EntityName}"
            _logger.LogInformation(
                "Counting {EntityName}",
                typeof(TEntity).Name);

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
                typeof(TEntity).Name);

            throw;
        }
    }

}

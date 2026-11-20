using Microsoft.EntityFrameworkCore.Storage;
using SportWearShop.Application.Abstractions.DataAccess;

namespace SportWearShop.Persistence.DataAccess;

internal sealed class EFUnitOfWork(StoreDbContext dbContext)
    : IUnitOfWork, IAsyncDisposable
{
    private IDbContextTransaction? _transaction;

    public bool HasActiveTransaction => _transaction is not null;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("A Store transaction is already active.");
        }

        _transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = _transaction
            ?? throw new InvalidOperationException("No active Store transaction to commit.");

        try
        {
            await dbContext.SaveChangesAsync(
                acceptAllChangesOnSuccess: false,
                cancellationToken: cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            dbContext.ChangeTracker.Clear();
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }

        dbContext.ChangeTracker.AcceptAllChanges();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not { } transaction)
        {
            return;
        }

        try
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            dbContext.ChangeTracker.Clear();
            await DisposeTransactionAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            dbContext.ChangeTracker.Clear();
            await DisposeTransactionAsync();
        }
    }

    private async ValueTask DisposeTransactionAsync()
    {
        var transaction = _transaction;
        _transaction = null;

        if (transaction is not null)
        {
            await transaction.DisposeAsync();
        }
    }
}

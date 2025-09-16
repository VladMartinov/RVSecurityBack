using Core.Interfaces;
using Persistence.Context;

namespace Persistence;

public class UnitOfWork(UserDbContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task StartTransactionAsync(CancellationToken cancellationToken = default)
    {
        await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = context.Database.CurrentTransaction;
        if (transaction == null)
            throw new NullReferenceException("Transaction cannot be null. You should start transaction before commiting transaction.");
        await transaction.CommitAsync(cancellationToken);
        await transaction.DisposeAsync();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = context.Database.CurrentTransaction;
        if (transaction == null)
            throw new NullReferenceException("Transaction cannot be null. You should start transaction before transaction rollback.");
        await transaction.RollbackAsync(cancellationToken);
        await transaction.DisposeAsync();
    }

    public async Task AddAsync<T>(T entity, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(entity ?? throw new ArgumentNullException(nameof(entity)), cancellationToken);
    }

    public async Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        where T : class
    {
        await context.Set<T>().AddRangeAsync(entities, cancellationToken);
    }

    public void Remove<T>(T entity)
    {
        context.Remove(entity ?? throw new ArgumentNullException(nameof(entity)));
    }

    public void RemoveRange<T>(IEnumerable<T> entities)
    {
        context.RemoveRange(entities);
    }
}
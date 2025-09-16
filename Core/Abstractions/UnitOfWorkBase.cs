using Core.Interfaces;

namespace Core.Abstractions;

public class UnitOfWorkBase : IUnitOfWork
{
    private readonly IUnitOfWork _unitOfWork;

    protected UnitOfWorkBase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public virtual Task AddAsync<T>(T entity, CancellationToken cancellationToken = default)
        => _unitOfWork.AddAsync(entity, cancellationToken);

    public virtual Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        where T : class
        => _unitOfWork.AddRangeAsync(entities, cancellationToken);

    public virtual void Remove<T>(T entity) => _unitOfWork.Remove(entity);

    public virtual void RemoveRange<T>(IEnumerable<T> entities) => _unitOfWork.RemoveRange(entities);

    public virtual Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _unitOfWork.SaveChangesAsync(cancellationToken);

    public virtual Task StartTransactionAsync(CancellationToken cancellationToken = default)
        => _unitOfWork.StartTransactionAsync(cancellationToken);

    public virtual Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        => _unitOfWork.CommitTransactionAsync(cancellationToken);

    public virtual Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        => _unitOfWork.RollbackTransactionAsync(cancellationToken);
}
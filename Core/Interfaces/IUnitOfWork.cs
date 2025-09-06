namespace Core.Interfaces;

public interface IUnitOfWork
{
    Task AddAsync<T>(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        where T : class;
    void Remove<T>(T entity);
    void RemoveRange<T>(IEnumerable<T> entities);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
using System.Linq.Expressions;

namespace DeveloperEvaluation.Core.Domain.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(int page = 1, int size = 10, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<T> Items, long TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>>? filter = null,
        string? orderBy = null,
        int page = 1,
        int size = 10,
        CancellationToken cancellationToken = default);
    Task<long> CountAsync(Expression<Func<T, bool>>? filter, CancellationToken cancellationToken = default);
}

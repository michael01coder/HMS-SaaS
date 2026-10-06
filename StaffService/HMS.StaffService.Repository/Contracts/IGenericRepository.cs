using System.Linq.Expressions;

namespace HMS.StaffService.Repository.Contracts
{
    public interface IGenericRepository<TEntity, TId> where TEntity : class
    {
        IQueryable<TEntity> Query();
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetAsync(TId id);
        Task<TResult?> GetAsync<TResult>(TId id);
        Task<TEntity> AddAsync(TEntity entity);
        Task<TResult> AddAsync<TSource, TResult>(TSource sourceDto);
        Task UpdateAsync(TId id, object dto);
        Task DeleteAsync(TId id);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
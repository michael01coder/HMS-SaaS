namespace HMS.AuthService.Repository.Contracts
{
    public interface IGenericRepository<T, TId> where T : class
    {
        IQueryable<T> Query();
        Task<List<T>> GetAllAsync();
        Task<T?> GetAsync(TId? id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(TId id);
        Task<bool> Exists(TId id);

        Task<List<TResult>> GetAllAsync<TResult>();
        Task<TResult?> GetAsync<TResult>(TId? id);
        Task<TResult> AddAsync<TSource, TResult>(TSource source);
        Task UpdateAsync<TSource>(TId id, TSource source);
    }

    // Default overload so existing entities using 'int' don't break
    public interface IGenericRepository<T> : IGenericRepository<T, int> where T : class
    {
    }
}
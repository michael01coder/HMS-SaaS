using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository.Contracts
{
    public interface IGenericRepository<TEntity , TId> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(object id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<int> SaveChangesAsync();
    }
}

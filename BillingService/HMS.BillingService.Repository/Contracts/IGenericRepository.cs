using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HMS.BillingService.Repository.Contracts
{
    public interface IGenericRepository<TEntity , TId> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TId id);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<bool> SaveChangesAsync();
    }
}

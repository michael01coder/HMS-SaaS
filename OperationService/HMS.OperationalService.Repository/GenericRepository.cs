using System.Linq.Expressions;
using HMS.OperationalService.Domain;
using HMS.OperationalService.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HMS.OperationalService.Repository
{
    public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : class
    {
        protected readonly OperationalDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(OperationalDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();

        public async Task<TEntity?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

        public void Update(TEntity entity) => _dbSet.Update(entity);

        public void Delete(TEntity entity) => _dbSet.Remove(entity);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}

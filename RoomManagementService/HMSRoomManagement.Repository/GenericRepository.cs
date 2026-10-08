using HMSRoomManagement.Domain;
using HMSRoomManagement.Repository.Contracts;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HMSRoomManagement.Repository
{
    public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : class
    {
        protected readonly HMSRoomManagementDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly IMapper _mapper;

        public GenericRepository(HMSRoomManagementDbContext context, IMapper mapper)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _mapper = mapper;
        }

        public IQueryable<TEntity> Query() => _dbSet.AsNoTracking();

        public async Task<List<TEntity>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

        public async Task<TEntity?> GetAsync(TId id) => await _dbSet.FindAsync(id);

        public async Task<TResult?> GetAsync<TResult>(TId id)
        {
            var entity = await GetAsync(id);
            return entity == null ? default : _mapper.Map<TResult>(entity);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TResult> AddAsync<TSource, TResult>(TSource sourceDto)
        {
            var entity = _mapper.Map<TEntity>(sourceDto);
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<TResult>(entity);
        }

        public async Task UpdateAsync(TId id, object dto)
        {
            var entity = await GetAsync(id);
            if (entity == null) return;

            dto.Adapt(entity, TypeAdapterConfig.GlobalSettings);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TId id)
        {
            var entity = await GetAsync(id);
            if (entity == null) return;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate) => await _dbSet.AnyAsync(predicate);
    }
}

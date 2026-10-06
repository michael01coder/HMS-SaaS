using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using HMS.AuthService.Domain.Data;
using HMS.AuthService.Repository.Contracts;
using HMS.AuthService.Domain;

namespace HMS.AuthService.Repository
{
    public class GenericRepository<T, TId> : IGenericRepository<T, TId> where T : class
    {
        protected readonly AuthServiceDbContext _authServiceDbContext;
        protected readonly DbSet<T> _dbSet;
        protected readonly IMapper _mapper;

        public GenericRepository(AuthServiceDbContext hotelDbContext, IMapper mapper)
        {
            _authServiceDbContext = hotelDbContext;
            _dbSet = _authServiceDbContext.Set<T>();
            _mapper = mapper;
        }

        public IQueryable<T> Query()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _authServiceDbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetAsync(TId? id)
        {
            if (id is null) return null;
            return await _authServiceDbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _authServiceDbContext.Set<T>().AddAsync(entity);
            await _authServiceDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _authServiceDbContext.Set<T>().Update(entity);
            await _authServiceDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TId id)
        {
            var entity = await GetAsync(id);
            if (entity is not null)
            {
                _authServiceDbContext.Set<T>().Remove(entity);
                await _authServiceDbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(TId id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }

        public async Task<List<TResult>> GetAllAsync<TResult>()
        {
            return await _authServiceDbContext.Set<T>()
                .ProjectToType<TResult>()
                .ToListAsync();
        }

        public async Task<TResult?> GetAsync<TResult>(TId? id)
        {
            var result = await GetAsync(id);
            if (result is null) return default;

            return _mapper.Map<TResult>(result);
        }

        public async Task<TResult> AddAsync<TSource, TResult>(TSource source)
        {
            var entity = _mapper.Map<T>(source);

            await _authServiceDbContext.Set<T>().AddAsync(entity);
            await _authServiceDbContext.SaveChangesAsync();

            return _mapper.Map<TResult>(entity);
        }

        public async Task UpdateAsync<TSource>(TId id, TSource source)
        {
            var entity = await GetAsync(id);
            if (entity is null) return;

            _mapper.Map(source, entity);
            _authServiceDbContext.Set<T>().Update(entity);
            await _authServiceDbContext.SaveChangesAsync();
        }
    }

    // Default overload so existing repositories using 'int' don't break
    public class GenericRepository<T> : GenericRepository<T, int>, IGenericRepository<T> where T : class
    {
        public GenericRepository(AuthServiceDbContext hotelDbContext, IMapper mapper)
            : base(hotelDbContext, mapper)
        {
        }
    }
}
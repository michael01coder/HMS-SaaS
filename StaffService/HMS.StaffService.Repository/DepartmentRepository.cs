using HMS.StaffService.Domain;
using HMS.StaffService.Domain.Data;
using HMS.StaffService.Repository.Contracts;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.StaffService.Repository
{
    public class DepartmentRepository : GenericRepository<Department, int>, IDepartmentRepository
    {
        public DepartmentRepository(StaffDbContext context, IMapper mapper)
            : base(context, mapper) { }

        public async Task<List<Department>> GetByHotelIdAsync(Guid hotelId)
        {
            return await _dbSet.AsNoTracking()
                .Where(d => d.HotelId == hotelId)
                .ToListAsync();
        }
    }
}
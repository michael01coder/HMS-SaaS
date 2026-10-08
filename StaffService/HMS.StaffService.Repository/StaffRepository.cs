using HMS.StaffService.Domain;
using HMS.StaffService.Domain.Data;
using HMS.StaffService.Repository.Contracts;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.StaffService.Repository
{
    public class StaffRepository : GenericRepository<Staff, Guid>, IStaffRepository
    {
        public StaffRepository(StaffDbContext context, IMapper mapper)
            : base(context, mapper) { }

        // Use AsNoTracking() strictly for GET requests / Read-only views
        public async Task<Staff?> GetWithDetailsAsync(Guid staffId)
        {
            return await _dbSet.AsNoTracking()
                .Include(s => s.Department)
                .Include(s => s.Manager)
                .Include(s => s.Documents)
                .Include(s => s.Notes)
                .FirstOrDefaultAsync(s => s.StaffId == staffId);
        }

        // Add a TRACKED version if you need inclusions during updates
        public async Task<Staff?> GetForUpdateAsync(Guid staffId)
        {
            return await _dbSet
                .Include(s => s.Department)
                .Include(s => s.Manager)
                .FirstOrDefaultAsync(s => s.StaffId == staffId);
        }

        public async Task<List<Staff>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _dbSet.AsNoTracking()
                .Where(s => s.DepartmentId == departmentId)
                .ToListAsync();
        }
    }
}
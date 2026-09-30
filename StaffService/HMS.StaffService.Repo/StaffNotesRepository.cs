using HMS.StaffService.Domain;
using HMS.StaffService.Domain.Data;
using HMS.StaffService.Repository.Contracts;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.StaffService.Repository
{
    public class StaffNotesRepository : GenericRepository<StaffNotes, Guid>, IStaffNotesRepository
    {
        public StaffNotesRepository(StaffDbContext context, IMapper mapper)
            : base(context, mapper) { }

        public async Task<List<StaffNotes>> GetByStaffIdAsync(Guid staffId)
        {
            return await _dbSet.AsNoTracking()
                .Where(n => n.StaffId == staffId)
                .ToListAsync();
        }
    }
}
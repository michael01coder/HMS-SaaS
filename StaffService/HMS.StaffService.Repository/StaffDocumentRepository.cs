using HMS.StaffService.Domain;
using HMS.StaffService.Domain.Data;
using HMS.StaffService.Repository.Contracts;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.StaffService.Repository
{
    public class StaffDocumentRepository : GenericRepository<StaffDocument, Guid>, IStaffDocumentRepository
    {
        public StaffDocumentRepository(StaffDbContext context, IMapper mapper)
            : base(context, mapper) { }

        public async Task<List<StaffDocument>> GetByStaffIdAsync(Guid staffId)
        {
            return await _dbSet.AsNoTracking()
                .Where(d => d.StaffId == staffId)
                .ToListAsync();
        }
    }
}
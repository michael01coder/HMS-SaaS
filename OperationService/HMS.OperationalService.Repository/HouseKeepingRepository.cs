using HMS.OperationalService.Domain;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository
{
    public class HouseKeepingRepository : GenericRepository<HouseKeepingTask , int>, IHouseKeepingRepository
    {
        public HouseKeepingRepository(OperationalDbContext context) : base(context) { }

        public async Task<IEnumerable<HouseKeepingTask>> GetTasksByStaffIdAsync(Guid staffId)
        {
            return await _dbSet.Where(t => t.AssignedStaffId == staffId).ToListAsync();
        }
    }
}

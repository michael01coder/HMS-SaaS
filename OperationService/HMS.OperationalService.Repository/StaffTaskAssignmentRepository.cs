using HMS.OperationalService.Domain;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository
{
    public class StaffTaskAssignmentRepository : GenericRepository<StaffTaskAssignment , Guid>, IStaffTaskAssignmentRepository
    {
        public StaffTaskAssignmentRepository(OperationalDbContext context) : base(context) { }

        public async Task<IEnumerable<StaffTaskAssignment>> GetAssignmentsByStaffAsync(Guid staffId)
        {
            return await _dbSet.Where(x => x.StaffId == staffId).ToListAsync();
        }
    }
}

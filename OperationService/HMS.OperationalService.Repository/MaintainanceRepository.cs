using HMS.AuthService.Repository;
using HMS.OperationalService.Domain;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository
{
    public class MaintainanceRepository : GenericRepository<MaintainanceRequest , Guid>, IMaintainanceRepository
    {
        public MaintainanceRepository(OperationalDbContext context) : base(context) { }

        public async Task<IEnumerable<MaintainanceRequest>> GetRequestsByTechnicianAsync(Guid technicianId)
        {
            return await _dbSet.Where(r => r.AssignedTechnicianId == technicianId).ToListAsync();
        }
    }
}


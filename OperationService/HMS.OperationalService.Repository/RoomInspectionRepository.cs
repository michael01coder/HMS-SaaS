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
    public class RoomInspectionRepository : GenericRepository<RoomInspection, int>, IRoomInspectionRepository
    {
        public RoomInspectionRepository(OperationalDbContext context) : base(context) { }

        public async Task<IEnumerable<RoomInspection>> GetInspectionsByRoomAsync(int roomId)
        {
            return await _dbSet.Where(i => i.RoomId == roomId).ToListAsync();
        }
    }
}

using HMS.AuthService.Repository;
using HMS.OperationalService.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository.Contracts
{
    public interface IRoomInspectionRepository : IGenericRepository<RoomInspection , int>
    {
        Task<IEnumerable<RoomInspection>> GetInspectionsByRoomAsync(int roomId);
    }
}

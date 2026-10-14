using HMS.OperationalService.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Service.Contracts
{
    public interface IRoomInspectionService
    {
        Task<IEnumerable<RoomInspection>> GetAllAsync();
        Task<RoomInspection> GetByIdAsync(int id);
        Task<RoomInspection> CreateAsync(RoomInspection inspection);
        Task DeleteAsync(int id);
    }
}

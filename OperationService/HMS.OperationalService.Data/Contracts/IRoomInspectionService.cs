using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;

namespace HMS.OperationalService.Service.Contracts
{
    public interface IRoomInspectionService
    {
        Task<IEnumerable<RoomInspection>> GetAllAsync();
        Task<RoomInspection> GetByIdAsync(int id);
        Task<RoomInspection> CreateAsync(RoomInspectionDto dto);
        Task DeleteAsync(int id);
    }
}

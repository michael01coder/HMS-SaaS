using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;

namespace HMS.OperationalService.Service.Contracts
{
    public interface IMaintainanceService
    {
        Task<IEnumerable<MaintainanceRequest>> GetAllAsync();
        Task<MaintainanceRequest> GetByIdAsync(int id);
        Task<MaintainanceRequest> CreateAsync(MaintenanceRequestDto dto);
        Task UpdateAsync(int id, MaintenanceRequestDto dto);
        Task DeleteAsync(int id);
    }
}

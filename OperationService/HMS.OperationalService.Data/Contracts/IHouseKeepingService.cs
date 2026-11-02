using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;

namespace HMS.OperationalService.Service.Contracts
{
    public interface IHouseKeepingService
    {
        Task<IEnumerable<HouseKeepingTask>> GetAllAsync();
        Task<HouseKeepingTask> GetByIdAsync(int id);
        Task<HouseKeepingTask> CreateAsync(HousekeepingTaskDto dto);
        Task UpdateAsync(int id, HousekeepingTaskDto dto);
        Task DeleteAsync(int id);
    }
}

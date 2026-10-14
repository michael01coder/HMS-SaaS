using HMS.OperationalService.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Service.Contracts
{
    public interface IHouseKeepingService
    {
        Task<IEnumerable<HouseKeepingTask>> GetAllTasksAsync();
        Task<HouseKeepingTask?> GetTaskByIdAsync(int id);
        Task<HouseKeepingTask> CreateTaskAsync(HouseKeepingTask task);
        Task UpdateTaskAsync(HouseKeepingTask task);
        Task DeleteTaskAsync(int id);
    }
}

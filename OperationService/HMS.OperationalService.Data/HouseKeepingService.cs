using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HMS.OperationalService.Data
{
    public class HouseKeepingService : IHouseKeepingService
    {
        private readonly IHouseKeepingRepository _repository;

        public HouseKeepingService(IHouseKeepingRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<HouseKeepingTask>> GetAllTasksAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<HouseKeepingTask?> GetTaskByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"Housekeeping task with ID {id} not found.");
        }

        public async Task<HouseKeepingTask> CreateTaskAsync(HouseKeepingTask task)
        {
            task.CreatedAt = DateTime.UtcNow;
            await _repository.AddAsync(task);
            await _repository.SaveChangesAsync();
            return task;
        }

        public async Task UpdateTaskAsync(HouseKeepingTask task)
        {
            task.UpdatedAt = DateTime.UtcNow;
            _repository.Update(task);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"Housekeeping task with ID {id} not found.");
            _repository.Delete(task);
            await _repository.SaveChangesAsync();
        }
    }
}


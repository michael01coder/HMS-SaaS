using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Common.Security;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using Status = HMS.OperationalService.Common.Enums.TaskStatus;

namespace HMS.OperationalService.Service
{
    public class HouseKeepingService : IHouseKeepingService
    {
        private readonly IHouseKeepingRepository _repository;
        private readonly IAccessGuard _guard;
        private readonly ICurrentUser _user;

        public HouseKeepingService(IHouseKeepingRepository repository, IAccessGuard guard, ICurrentUser user)
        {
            _repository = repository;
            _guard = guard;
            _user = user;
        }

        public async Task<IEnumerable<HouseKeepingTask>> GetAllAsync()
        {
            _guard.Require(Module.Housekeeping);
            var hotelId = _guard.HotelId;
            var items = await _repository.FindAsync(t => t.HotelId == hotelId);
            if (_user.Role == "Staff") items = items.Where(t => t.AssignedStaffId == _user.StaffId);
            return items.OrderByDescending(t => t.ScheduledDate).ToList();
        }

        public async Task<HouseKeepingTask> GetByIdAsync(int id)
        {
            _guard.Require(Module.Housekeeping);
            var task = await GetOwnedAsync(id);
            if (_user.Role == "Staff" && task.AssignedStaffId != _user.StaffId)
                throw new ForbiddenException("This task is assigned to someone else.");
            return task;
        }

        public async Task<HouseKeepingTask> CreateAsync(HousekeepingTaskDto dto)
        {
            _guard.Require(Module.Housekeeping);
            _guard.RequireRole("HotelOwner", "HR", "Manager");

            var task = new HouseKeepingTask
            {
                HotelId = _guard.HotelId,
                TaskCode = "HK-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(),
                RoomId = dto.RoomId,
                TaskType = dto.TaskType,
                AssignedStaffId = dto.AssignedStaffId,
                Priority = dto.Priority,
                Status = Status.Pending,
                Remarks = dto.Remarks,
                ScheduledDate = Utc.From(dto.ScheduledDate),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(task);
            await _repository.SaveChangesAsync();
            return task;
        }

        public async Task UpdateAsync(int id, HousekeepingTaskDto dto)
        {
            _guard.Require(Module.Housekeeping);
            var task = await GetOwnedAsync(id);

            if (_user.Role == "Staff")
            {
                if (task.AssignedStaffId != _user.StaffId)
                    throw new ForbiddenException("This task is assigned to someone else.");
            }
            else
            {
                task.RoomId = dto.RoomId;
                task.TaskType = dto.TaskType;
                task.AssignedStaffId = dto.AssignedStaffId;
                task.Priority = dto.Priority;
                task.ScheduledDate = Utc.From(dto.ScheduledDate);
            }

            task.Remarks = dto.Remarks;
            if (task.Status != dto.Status)
            {
                task.Status = dto.Status;
                if (dto.Status == Status.InProgress) task.StartedAt ??= DateTime.UtcNow;
                if (dto.Status == Status.Completed) task.CompletedAt = DateTime.UtcNow;
            }
            task.UpdatedAt = DateTime.UtcNow;

            _repository.Update(task);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _guard.Require(Module.Housekeeping);
            _guard.RequireRole("HotelOwner", "HR", "Manager");
            _repository.Delete(await GetOwnedAsync(id));
            await _repository.SaveChangesAsync();
        }

        private async Task<HouseKeepingTask> GetOwnedAsync(int id)
        {
            var task = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Housekeeping task {id} not found.");
            if (task.HotelId != _guard.HotelId) throw new ForbiddenException("Access denied.");
            return task;
        }
    }
}

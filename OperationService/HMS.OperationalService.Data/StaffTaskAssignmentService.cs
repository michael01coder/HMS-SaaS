using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Common.Security;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using Status = HMS.OperationalService.Common.Enums.TaskStatus;

namespace HMS.OperationalService.Service
{
    public class StaffTaskAssignmentService : IStaffTaskAssignmentService
    {
        private readonly IStaffTaskAssignmentRepository _repository;
        private readonly IAccessGuard _guard;
        private readonly ICurrentUser _user;

        public StaffTaskAssignmentService(IStaffTaskAssignmentRepository repository, IAccessGuard guard, ICurrentUser user)
        {
            _repository = repository;
            _guard = guard;
            _user = user;
        }

        private void RequireAccess() => _guard.RequireAny(Module.Housekeeping, Module.Maintenance, Module.FrontDesk);

        public async Task<IEnumerable<StaffTaskAssignment>> GetAllAsync()
        {
            RequireAccess();
            var hotelId = _guard.HotelId;
            var items = await _repository.FindAsync(a => a.HotelId == hotelId);
            if (_user.Role == "Staff") items = items.Where(a => a.StaffId == _user.StaffId);
            return items.OrderByDescending(a => a.AssignedAt).ToList();
        }

        public async Task<StaffTaskAssignment> GetByIdAsync(int id)
        {
            RequireAccess();
            var assignment = await GetOwnedAsync(id);
            if (_user.Role == "Staff" && assignment.StaffId != _user.StaffId)
                throw new ForbiddenException("This assignment belongs to someone else.");
            return assignment;
        }

        public async Task<StaffTaskAssignment> CreateAsync(StaffTaskAssignmentDto dto)
        {
            RequireAccess();
            _guard.RequireRole("HotelOwner", "HR", "Manager");

            var assignment = new StaffTaskAssignment
            {
                HotelId = _guard.HotelId,
                StaffId = dto.StaffId,
                TaskType = dto.TaskType,
                TaskReferenceId = dto.TaskReferenceId,
                AssignedBy = _user.StaffId ?? dto.AssignedBy,
                AssignedAt = DateTime.UtcNow,
                Status = Status.Pending,
                Remarks = dto.Remarks
            };

            await _repository.AddAsync(assignment);
            await _repository.SaveChangesAsync();
            return assignment;
        }

        public async Task UpdateAsync(int id, StaffTaskAssignmentDto dto)
        {
            RequireAccess();
            var assignment = await GetOwnedAsync(id);

            if (_user.Role == "Staff")
            {
                if (assignment.StaffId != _user.StaffId)
                    throw new ForbiddenException("This assignment belongs to someone else.");
            }
            else
            {
                assignment.StaffId = dto.StaffId;
                assignment.TaskType = dto.TaskType;
                assignment.TaskReferenceId = dto.TaskReferenceId;
            }

            assignment.Status = dto.Status;
            assignment.Remarks = dto.Remarks;

            _repository.Update(assignment);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            RequireAccess();
            _guard.RequireRole("HotelOwner", "HR", "Manager");
            _repository.Delete(await GetOwnedAsync(id));
            await _repository.SaveChangesAsync();
        }

        private async Task<StaffTaskAssignment> GetOwnedAsync(int id)
        {
            var assignment = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Assignment {id} not found.");
            if (assignment.HotelId != _guard.HotelId) throw new ForbiddenException("Access denied.");
            return assignment;
        }
    }
}

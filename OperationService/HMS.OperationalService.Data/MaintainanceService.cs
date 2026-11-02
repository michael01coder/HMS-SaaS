using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Common.Security;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using Status = HMS.OperationalService.Common.Enums.TaskStatus;

namespace HMS.OperationalService.Service
{
    public class MaintainanceService : IMaintainanceService
    {
        private readonly IMaintainanceRepository _repository;
        private readonly IAccessGuard _guard;
        private readonly ICurrentUser _user;

        public MaintainanceService(IMaintainanceRepository repository, IAccessGuard guard, ICurrentUser user)
        {
            _repository = repository;
            _guard = guard;
            _user = user;
        }

        private void RequireAccess() => _guard.RequireAny(Module.Maintenance, Module.Housekeeping, Module.FrontDesk);

        public async Task<IEnumerable<MaintainanceRequest>> GetAllAsync()
        {
            RequireAccess();
            var hotelId = _guard.HotelId;
            var items = await _repository.FindAsync(r => r.HotelId == hotelId);
            if (_user.Role == "Staff")
                items = items.Where(r => r.AssignedTechnicianId == _user.StaffId || r.ReportedBy == _user.StaffId);
            return items.OrderByDescending(r => r.CreatedAt).ToList();
        }

        public async Task<MaintainanceRequest> GetByIdAsync(int id)
        {
            RequireAccess();
            var request = await GetOwnedAsync(id);
            if (_user.Role == "Staff" && request.AssignedTechnicianId != _user.StaffId && request.ReportedBy != _user.StaffId)
                throw new ForbiddenException("You can only view requests you reported or were assigned.");
            return request;
        }

        public async Task<MaintainanceRequest> CreateAsync(MaintenanceRequestDto dto)
        {
            RequireAccess();

            var request = new MaintainanceRequest
            {
                HotelId = _guard.HotelId,
                RequestCode = "MNT-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(),
                RoomId = dto.RoomId,
                Category = dto.Category,
                IssueTitle = dto.IssueTitle,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = Status.Pending,
                AssignedTechnicianId = _user.Role == "Staff" ? null : dto.AssignedTechnicianId,
                ReportedBy = _user.StaffId ?? dto.ReportedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(request);
            await _repository.SaveChangesAsync();
            return request;
        }

        public async Task UpdateAsync(int id, MaintenanceRequestDto dto)
        {
            RequireAccess();
            var request = await GetOwnedAsync(id);

            if (_user.Role == "Staff")
            {
                if (request.AssignedTechnicianId != _user.StaffId)
                    throw new ForbiddenException("Only the assigned technician can update this request.");
            }
            else
            {
                request.RoomId = dto.RoomId;
                request.Category = dto.Category;
                request.IssueTitle = dto.IssueTitle;
                request.Description = dto.Description;
                request.Priority = dto.Priority;
                request.AssignedTechnicianId = dto.AssignedTechnicianId;
            }

            request.Status = dto.Status;
            if (dto.Status == Status.Resolved) request.ResolvedAt ??= DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;

            _repository.Update(request);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            RequireAccess();
            _guard.RequireRole("HotelOwner", "HR", "Manager");
            _repository.Delete(await GetOwnedAsync(id));
            await _repository.SaveChangesAsync();
        }

        private async Task<MaintainanceRequest> GetOwnedAsync(int id)
        {
            var request = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Maintenance request {id} not found.");
            if (request.HotelId != _guard.HotelId) throw new ForbiddenException("Access denied.");
            return request;
        }
    }
}

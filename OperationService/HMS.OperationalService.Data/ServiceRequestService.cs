using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Common.Security;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using Status = HMS.OperationalService.Common.Enums.TaskStatus;

namespace HMS.OperationalService.Service
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _repository;
        private readonly IAccessGuard _guard;

        public ServiceRequestService(IServiceRequestRepository repository, IAccessGuard guard)
        {
            _repository = repository;
            _guard = guard;
        }

        private void RequireAccess() => _guard.RequireAny(Module.FrontDesk, Module.Housekeeping);

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
        {
            RequireAccess();
            var hotelId = _guard.HotelId;
            var items = await _repository.FindAsync(r => r.HotelId == hotelId);
            return items.OrderByDescending(r => r.RequestedAt).ToList();
        }

        public async Task<ServiceRequest> GetByIdAsync(int id)
        {
            RequireAccess();
            return await GetOwnedAsync(id);
        }

        public async Task<ServiceRequest> CreateAsync(ServiceRequestDto dto)
        {
            RequireAccess();

            var request = new ServiceRequest
            {
                HotelId = _guard.HotelId,
                RequestCode = "SRV-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(),
                GuestId = dto.GuestId,
                RoomId = dto.RoomId,
                RequestType = dto.RequestType,
                Description = dto.Description,
                AssignedStaffId = dto.AssignedStaffId,
                Status = Status.Pending,
                RequestedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(request);
            await _repository.SaveChangesAsync();
            return request;
        }

        public async Task UpdateAsync(int id, ServiceRequestDto dto)
        {
            RequireAccess();
            var request = await GetOwnedAsync(id);

            request.RoomId = dto.RoomId;
            request.RequestType = dto.RequestType;
            request.Description = dto.Description;
            request.AssignedStaffId = dto.AssignedStaffId;
            request.Status = dto.Status;
            if (dto.Status == Status.Completed) request.CompletedAt ??= DateTime.UtcNow;
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

        private async Task<ServiceRequest> GetOwnedAsync(int id)
        {
            var request = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Service request {id} not found.");
            if (request.HotelId != _guard.HotelId) throw new ForbiddenException("Access denied.");
            return request;
        }
    }
}

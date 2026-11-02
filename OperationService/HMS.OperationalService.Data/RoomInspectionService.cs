using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Common.Security;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using Status = HMS.OperationalService.Common.Enums.TaskStatus;

namespace HMS.OperationalService.Service
{
    public class RoomInspectionService : IRoomInspectionService
    {
        private readonly IRoomInspectionRepository _repository;
        private readonly IAccessGuard _guard;
        private readonly ICurrentUser _user;

        public RoomInspectionService(IRoomInspectionRepository repository, IAccessGuard guard, ICurrentUser user)
        {
            _repository = repository;
            _guard = guard;
            _user = user;
        }

        public async Task<IEnumerable<RoomInspection>> GetAllAsync()
        {
            _guard.Require(Module.Housekeeping);
            var hotelId = _guard.HotelId;
            var items = await _repository.FindAsync(i => i.HotelId == hotelId);
            return items.OrderByDescending(i => i.InspectionDate).ToList();
        }

        public async Task<RoomInspection> GetByIdAsync(int id)
        {
            _guard.Require(Module.Housekeeping);
            return await GetOwnedAsync(id);
        }

        public async Task<RoomInspection> CreateAsync(RoomInspectionDto dto)
        {
            _guard.Require(Module.Housekeeping);

            var inspection = new RoomInspection
            {
                HotelId = _guard.HotelId,
                InspectionCode = "INS-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(),
                RoomId = dto.RoomId,
                InspectorId = _user.StaffId ?? dto.InspectorId,
                InspectionType = dto.InspectionType,
                ConditionRating = dto.ConditionRating,
                Remarks = dto.Remarks,
                InspectionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(inspection);
            await _repository.SaveChangesAsync();
            return inspection;
        }

        public async Task DeleteAsync(int id)
        {
            _guard.Require(Module.Housekeeping);
            _guard.RequireRole("HotelOwner", "HR", "Manager");
            _repository.Delete(await GetOwnedAsync(id));
            await _repository.SaveChangesAsync();
        }

        private async Task<RoomInspection> GetOwnedAsync(int id)
        {
            var inspection = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Inspection {id} not found.");
            if (inspection.HotelId != _guard.HotelId) throw new ForbiddenException("Access denied.");
            return inspection;
        }
    }
}

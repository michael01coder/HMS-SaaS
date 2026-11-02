using HMSRoomManagement.Common.Constants;
using HMSRoomManagement.Common.Exceptions;
using HMSRoomManagement.Common.Service;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.DTO.Room;
using HMSRoomManagement.Repository.Contracts;
using HMSRoomManagement.Service.Contracts;

namespace HMSRoomManagement.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ICurrentUserService _currentUserService;

        public RoomService(IRoomRepository roomRepository, ICurrentUserService currentUserService)
        {
            _roomRepository = roomRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<RoomDto>> GetRoomsAsync(Guid? hotelId, int? roomTypeId)
        {
            var rooms = await _roomRepository.GetAllAsync();
            var scoped = ScopedHotelId(hotelId);
            if (scoped.HasValue) rooms = rooms.Where(r => r.HotelId == scoped.Value).ToList();
            if (roomTypeId.HasValue) rooms = rooms.Where(r => r.RoomTypeId == roomTypeId.Value).ToList();

            return rooms.Select(MapToDto);
        }

        public async Task<RoomDto> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepository.GetAsync(id)
                ?? throw new NotFoundException($"Room {id} not found.");

            EnsureCanRead(room.HotelId);
            return MapToDto(room);
        }

        public async Task<RoomDto> CreateRoomAsync(CreateRoomDto dto)
        {
            EnsureManagementScope(dto.HotelId);
            return await _roomRepository.AddAsync<CreateRoomDto, RoomDto>(dto);
        }

        public async Task UpdateRoomAsync(int id, UpdateRoomDto dto)
        {
            var room = await _roomRepository.GetAsync(id)
                ?? throw new NotFoundException($"Room {id} not found.");

            EnsureManagementScope(room.HotelId);
            await _roomRepository.UpdateAsync(id, dto);
        }

        public async Task UpdateRoomStatusAsync(int id, string status)
        {
            var room = await _roomRepository.GetAsync(id)
                ?? throw new NotFoundException($"Room {id} not found.");

            var role = _currentUserService.Role;
            var sameHotel = _currentUserService.HotelId == room.HotelId;

            bool allowed =
                role is Roles.SuperAdmin or Roles.TenantAdmin ||
                (role == Roles.Manager && sameHotel) ||
                (role is Roles.HR or Roles.HotelOwner && sameHotel) ||
                (role == Roles.Staff && sameHotel && IsRoomDesk(_currentUserService.Department));

            if (!allowed)
                throw new ForbiddenException("Only hotel management, Front Desk or Housekeeping staff can update room status.");

            await _roomRepository.UpdateAsync(id, new { Status = status });
        }

        public async Task DeleteRoomAsync(int id)
        {
            var room = await _roomRepository.GetAsync(id)
                ?? throw new NotFoundException($"Room {id} not found.");

            EnsureManagementScope(room.HotelId);
            await _roomRepository.DeleteAsync(id);
        }


        private bool IsPlatformScope => _currentUserService.Role is Roles.SuperAdmin or Roles.TenantAdmin;

        private Guid? ScopedHotelId(Guid? requested)
        {
            if (IsPlatformScope) return requested;
            var own = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId claim.");
            if (requested.HasValue && requested.Value != own)
                throw new ForbiddenException("You don't have access to this hotel.");
            return own;
        }

        private void EnsureCanRead(Guid hotelId)
        {
            if (!IsPlatformScope && _currentUserService.HotelId != hotelId)
                throw new ForbiddenException("You don't have access to this hotel.");
        }

        private static bool IsRoomDesk(string? department) =>
            department != null &&
            (department.Contains("front", StringComparison.OrdinalIgnoreCase)
             || department.Contains("housekeep", StringComparison.OrdinalIgnoreCase));

        private void EnsureManagementScope(Guid hotelId)
        {
            var role = _currentUserService.Role;
            if (role is Roles.SuperAdmin or Roles.TenantAdmin) return;
            if (role is Roles.HotelOwner or Roles.HR && _currentUserService.HotelId == hotelId) return;

            throw new ForbiddenException("You don't have permission to manage rooms for this hotel.");
        }

        private static RoomDto MapToDto(Room room) => new()
        {
            Id = room.Id,
            HotelId = room.HotelId,
            RoomTypeId = room.RoomTypeId,
            RoomNumber = room.RoomNumber,
            FloorNumber = room.FloorNumber,
            Status = room.Status,
            IsActive = room.IsActive
        };
    }
}

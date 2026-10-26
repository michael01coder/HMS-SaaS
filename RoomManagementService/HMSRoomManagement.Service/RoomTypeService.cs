using HMSRoomManagement.Common.Constants;
using HMSRoomManagement.Common.Exceptions;
using HMSRoomManagement.Common.Service;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.DTO.RoomType;
using HMSRoomManagement.Repository.Contracts;
using HMSRoomManagement.Service.Contracts;

namespace HMSRoomManagement.Service
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly ICurrentUserService _currentUserService;

        public RoomTypeService(IRoomTypeRepository roomTypeRepository, ICurrentUserService currentUserService)
        {
            _roomTypeRepository = roomTypeRepository;
            _currentUserService = currentUserService;
        }

        // Room types are read broadly — every role that can see a hotel at
        // all needs to see its room types (front desk, housekeeping, etc.),
        // so this isn't scoped the way writes are.
        public async Task<IEnumerable<RoomTypeDto>> GetRoomTypesAsync(Guid? hotelId)
        {
            var roomTypes = await _roomTypeRepository.GetAllAsync();
            if (hotelId.HasValue)
                roomTypes = roomTypes.Where(rt => rt.HotelId == hotelId.Value).ToList();

            return roomTypes.Select(MapToDto);
        }

        public async Task<RoomTypeDto> GetRoomTypeByIdAsync(int id)
        {
            var roomType = await _roomTypeRepository.GetAsync(id)
                ?? throw new NotFoundException($"Room type {id} not found.");

            return MapToDto(roomType);
        }

        public async Task<RoomTypeDto> CreateRoomTypeAsync(CreateRoomTypeDto dto)
        {
            EnsureHotelScope(dto.HotelId);
            return await _roomTypeRepository.AddAsync<CreateRoomTypeDto, RoomTypeDto>(dto);
        }

        public async Task UpdateRoomTypeAsync(int id, UpdateRoomTypeDto dto)
        {
            var roomType = await _roomTypeRepository.GetAsync(id)
                ?? throw new NotFoundException($"Room type {id} not found.");

            EnsureHotelScope(roomType.HotelId);
            await _roomTypeRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteRoomTypeAsync(int id)
        {
            var roomType = await _roomTypeRepository.GetAsync(id)
                ?? throw new NotFoundException($"Room type {id} not found.");

            EnsureHotelScope(roomType.HotelId);
            await _roomTypeRepository.DeleteAsync(id);
        }

        /// <summary>
        /// SuperAdmin/TenantAdmin can manage room types at any hotel (this
        /// service has no TenantId on its own records to check against, so
        /// TenantAdmin is trusted at this tier — Hotel Service is where
        /// TenantId-level scoping is enforced). HotelOwner/Manager are
        /// scoped to their own single hotel.
        /// </summary>
        private void EnsureHotelScope(Guid hotelId)
        {
            var role = _currentUserService.Role;
            if (role is Roles.SuperAdmin or Roles.TenantAdmin) return;
            if (role is Roles.HotelOwner or Roles.HR && _currentUserService.HotelId == hotelId) return;

            throw new ForbiddenException("You don't have permission to manage room types for this hotel.");
        }

        private static RoomTypeDto MapToDto(RoomType rt) => new()
        {
            Id = rt.Id,
            HotelId = rt.HotelId,
            TypeName = rt.TypeName,
            Description = rt.Description,
            MaxAdults = rt.MaxAdults,
            MaxChildren = rt.MaxChildren,
            SizeSqft = rt.SizeSqft,
            BedType = rt.BedType,
            BasePricePerNight = rt.BasePricePerNight
        };
    }
}

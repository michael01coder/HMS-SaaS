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

        public async Task<IEnumerable<RoomTypeDto>> GetRoomTypesAsync(Guid? hotelId)
        {
            var roomTypes = await _roomTypeRepository.GetAllAsync();
            var scoped = ScopedHotelId(hotelId);
            if (scoped.HasValue)
                roomTypes = roomTypes.Where(rt => rt.HotelId == scoped.Value).ToList();

            return roomTypes.Select(MapToDto);
        }

        public async Task<RoomTypeDto> GetRoomTypeByIdAsync(int id)
        {
            var roomType = await _roomTypeRepository.GetAsync(id)
                ?? throw new NotFoundException($"Room type {id} not found.");

            EnsureCanRead(roomType.HotelId);
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

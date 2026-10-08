using HMSRoomManagement.Common.Constants;
using HMSRoomManagement.Common.Exceptions;
using HMSRoomManagement.Common.Service;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.DTO.RoomTypeAmenity;
using HMSRoomManagement.Repository.Contracts;
using HMSRoomManagement.Service.Contracts;

namespace HMSRoomManagement.Service
{
    public class RoomTypeAmenityService : IRoomTypeAmenityService
    {
        private readonly IRoomTypeAmenityRepository _repository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly ICurrentUserService _currentUserService;

        public RoomTypeAmenityService(
            IRoomTypeAmenityRepository repository,
            IRoomTypeRepository roomTypeRepository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _roomTypeRepository = roomTypeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<RoomTypeAmenityDto>> GetLinksAsync(int? roomTypeId)
        {
            var links = roomTypeId.HasValue
                ? await _repository.GetByRoomTypeAsync(roomTypeId.Value)
                : await _repository.GetAllAsync();

            return links.Select(l => new RoomTypeAmenityDto { RoomTypeId = l.RoomTypeId, AmenityId = l.AmenityId });
        }

        public async Task<RoomTypeAmenityDto> LinkAsync(RoomTypeAmenityDto dto)
        {
            await EnsureHotelScopeAsync(dto.RoomTypeId);

            var existing = await _repository.FindAsync(dto.RoomTypeId, dto.AmenityId);
            if (existing != null)
                throw new BadRequestException("This amenity is already linked to this room type.");

            var link = new RoomTypeAmenity { RoomTypeId = dto.RoomTypeId, AmenityId = dto.AmenityId };
            await _repository.AddAsync(link);
            await _repository.SaveChangesAsync();

            return dto;
        }

        public async Task UnlinkAsync(int roomTypeId, int amenityId)
        {
            await EnsureHotelScopeAsync(roomTypeId);

            var existing = await _repository.FindAsync(roomTypeId, amenityId)
                ?? throw new NotFoundException("That link doesn't exist.");

            _repository.Remove(existing);
            await _repository.SaveChangesAsync();
        }

        private async Task EnsureHotelScopeAsync(int roomTypeId)
        {
            var role = _currentUserService.Role;
            if (role is Roles.SuperAdmin or Roles.TenantAdmin) return;

            var roomType = await _roomTypeRepository.GetAsync(roomTypeId)
                ?? throw new NotFoundException($"Room type {roomTypeId} not found.");

            if (role is Roles.HotelOwner or Roles.Manager && _currentUserService.HotelId == roomType.HotelId) return;

            throw new ForbiddenException("You don't have permission to manage amenities for this room type.");
        }
    }
}

using HMSHotel.Common.Constants;
using HMSHotel.Common.Exceptions;
using HMSHotel.Common.Service;
using HMSHotel.Domain.Data;
using HMSHotel.DTO.HotelAmenity;
using HMSHotel.Repository.Contracts;
using HMSHotel.Service.Contracts;

namespace HMSHotel.Service
{
    public class HotelAmenityService : IHotelAmenityService
    {
        private readonly IHotelAmenityRepository _repository;
        private readonly IHotelRepository _hotels;
        private readonly ICurrentUserService _currentUser;

        public HotelAmenityService(IHotelAmenityRepository repository, IHotelRepository hotels, ICurrentUserService currentUser)
        {
            _repository = repository;
            _hotels = hotels;
            _currentUser = currentUser;
        }

        private async Task EnsureScopeAsync(Guid hotelId, bool write)
        {
            var hotel = await _hotels.GetAsync(hotelId)
                ?? throw new NotFoundException($"Hotel '{hotelId}' not found.");

            var role = _currentUser.Role;
            if (role == Roles.SuperAdmin) return;
            if (role == Roles.TenantAdmin && hotel.TenantId == _currentUser.TenantId) return;
            if (hotel.Id == _currentUser.HotelId && (!write || role is Roles.HotelOwner or Roles.HR)) return;

            throw new ForbiddenException("You don't have access to this hotel.");
        }

        public async Task<IEnumerable<HotelAmenityDto>> GetLinksAsync(Guid? hotelId)
        {
            var scopeId = hotelId ?? (_currentUser.Role is Roles.SuperAdmin or Roles.TenantAdmin ? null : _currentUser.HotelId);

            if (scopeId.HasValue)
            {
                await EnsureScopeAsync(scopeId.Value, false);
                var scoped = await _repository.GetByHotelAsync(scopeId.Value);
                return scoped.Select(l => new HotelAmenityDto { HotelId = l.HotelId, AmenityId = l.AmenityId });
            }

            var allowed = (await _hotels.GetAllAsync())
                .Where(h => _currentUser.Role == Roles.SuperAdmin || h.TenantId == _currentUser.TenantId)
                .Select(h => h.Id)
                .ToHashSet();
            var links = (await _repository.GetAllAsync()).Where(l => allowed.Contains(l.HotelId));

            return links.Select(l => new HotelAmenityDto { HotelId = l.HotelId, AmenityId = l.AmenityId });
        }

        public async Task<HotelAmenityDto> LinkAsync(HotelAmenityDto dto)
        {
            await EnsureScopeAsync(dto.HotelId, true);
            var existing = await _repository.FindAsync(dto.HotelId, dto.AmenityId);
            if (existing != null)
                throw new BadRequestException("This amenity is already linked to this hotel.");

            var link = new HotelAmenity { HotelId = dto.HotelId, AmenityId = dto.AmenityId };
            await _repository.AddAsync(link);
            await _repository.SaveChangesAsync();

            return dto;
        }

        public async Task UnlinkAsync(Guid hotelId, int amenityId)
        {
            await EnsureScopeAsync(hotelId, true);
            var existing = await _repository.FindAsync(hotelId, amenityId)
                ?? throw new NotFoundException("That link doesn't exist.");

            _repository.Remove(existing);
            await _repository.SaveChangesAsync();
        }
    }
}

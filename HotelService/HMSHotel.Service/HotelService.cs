using HMSHotel.Common.Constants;
using HMSHotel.Common.Exceptions;
using HMSHotel.Common.Service;
using HMSHotel.Domain.Data;
using HMSHotel.DTO.Hotel;
using HMSHotel.Repository.Contracts;
using HMSHotel.Service.Contracts;

namespace HMSHotel.Service
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly ICurrentUserService _currentUserService;

        public HotelService(IHotelRepository hotelRepository, ICurrentUserService currentUserService)
        {
            _hotelRepository = hotelRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<HotelDto>> GetAllHotelsAsync()
        {
            var hotels = await _hotelRepository.GetAllAsync();
            var role = _currentUserService.Role;

            // SuperAdmin sees every hotel platform-wide.
            if (role == Roles.SuperAdmin)
                return hotels.Select(MapToDto);

            // TenantAdmin sees every hotel under their own tenant.
            if (role == Roles.TenantAdmin)
                return hotels.Where(h => h.TenantId == _currentUserService.TenantId).Select(MapToDto);

            // HotelOwner/HR/Manager/Staff only ever see their own single hotel.
            return hotels.Where(h => h.Id == _currentUserService.HotelId).Select(MapToDto);
        }

        public async Task<HotelDto> GetHotelByIdAsync(Guid id)
        {
            var hotel = await _hotelRepository.GetAsync(id)
                ?? throw new NotFoundException($"Hotel '{id}' not found.");

            EnsureReadScope(hotel);

            return MapToDto(hotel);
        }

        public async Task<HotelDto> CreateHotelAsync(CreateHotelDto dto)
        {
            // SuperAdmin can create under any tenant; TenantAdmin only under
            // their own — HotelOwner/HR/Manager/Staff can't create hotels at
            // all (not reachable here — gated at the controller).
            if (_currentUserService.Role == Roles.TenantAdmin && dto.TenantId != _currentUserService.TenantId)
                throw new ForbiddenException("You can only create hotels under your own tenant.");

            return await _hotelRepository.AddAsync<CreateHotelDto, HotelDto>(dto);
        }

        public async Task UpdateHotelAsync(Guid id, UpdateHotelDto dto)
        {
            var hotel = await _hotelRepository.GetAsync(id)
                ?? throw new NotFoundException($"Hotel '{id}' not found.");

            EnsureWriteScope(hotel);

            await _hotelRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteHotelAsync(Guid id)
        {
            var hotel = await _hotelRepository.GetAsync(id)
                ?? throw new NotFoundException($"Hotel '{id}' not found.");

            // Only SuperAdmin or the owning TenantAdmin may delete — a
            // HotelOwner managing their own hotel still can't delete it.
            if (_currentUserService.Role == Roles.TenantAdmin && hotel.TenantId != _currentUserService.TenantId)
                throw new ForbiddenException("You can only delete hotels under your own tenant.");

            await _hotelRepository.DeleteAsync(id);
        }

        private void EnsureReadScope(Hotel hotel)
        {
            var role = _currentUserService.Role;
            if (role == Roles.SuperAdmin) return;
            if (role == Roles.TenantAdmin && hotel.TenantId == _currentUserService.TenantId) return;
            if (hotel.Id == _currentUserService.HotelId) return;

            throw new ForbiddenException("You don't have access to this hotel.");
        }

        private void EnsureWriteScope(Hotel hotel)
        {
            var role = _currentUserService.Role;
            if (role == Roles.SuperAdmin) return;
            if (role == Roles.TenantAdmin && hotel.TenantId == _currentUserService.TenantId) return;
            if (role == Roles.HotelOwner && hotel.Id == _currentUserService.HotelId) return;

            throw new ForbiddenException("You don't have permission to edit this hotel.");
        }

        private static HotelDto MapToDto(Hotel hotel) => new()
        {
            Id = hotel.Id,
            TenantId = hotel.TenantId,
            Name = hotel.Name,
            Code = hotel.Code,
            PropertyType = hotel.PropertyType,
            StarRating = hotel.StarRating,
            AddressLine1 = hotel.AddressLine1,
            City = hotel.City,
            State = hotel.State,
            Country = hotel.Country,
            Latitude = hotel.Latitude,
            Longitude = hotel.Longitude,
            Phone = hotel.Phone,
            Email = hotel.Email,
            Description = hotel.Description,
            TotalFloors = hotel.TotalFloors,
            IsActive = hotel.IsActive,
            CreatedAt = hotel.CreatedAt
        };
    }
}

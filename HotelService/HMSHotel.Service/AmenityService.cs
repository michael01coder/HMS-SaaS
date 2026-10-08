using HMSHotel.Common.Exceptions;
using HMSHotel.DTO.Amenity;
using HMSHotel.Repository.Contracts;
using HMSHotel.Service.Contracts;

namespace HMSHotel.Service
{
    public class AmenityService : IAmenityService
    {
        private readonly IAmenityRepository _amenityRepository;

        public AmenityService(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        // Amenities are platform/tenant-level master data, not hotel-scoped —
        // any authenticated role can read them (enforced at the controller
        // with a plain [Authorize], no extra scope check needed here).
        public async Task<IEnumerable<AmenityDto>> GetAmenitiesAsync(string? category)
        {
            var amenities = await _amenityRepository.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(category))
                amenities = amenities.Where(a => a.Category == category).ToList();

            return amenities.Select(a => new AmenityDto { Id = a.Id, Name = a.Name, Category = a.Category, Icon = a.Icon });
        }

        public async Task<AmenityDto> GetAmenityByIdAsync(int id)
        {
            var amenity = await _amenityRepository.GetAsync(id)
                ?? throw new NotFoundException($"Amenity {id} not found.");

            return new AmenityDto { Id = amenity.Id, Name = amenity.Name, Category = amenity.Category, Icon = amenity.Icon };
        }

        public async Task<AmenityDto> CreateAmenityAsync(CreateAmenityDto dto)
        {
            return await _amenityRepository.AddAsync<CreateAmenityDto, AmenityDto>(dto);
        }

        public async Task UpdateAmenityAsync(int id, UpdateAmenityDto dto)
        {
            _ = await _amenityRepository.GetAsync(id) ?? throw new NotFoundException($"Amenity {id} not found.");
            await _amenityRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteAmenityAsync(int id)
        {
            _ = await _amenityRepository.GetAsync(id) ?? throw new NotFoundException($"Amenity {id} not found.");
            await _amenityRepository.DeleteAsync(id);
        }
    }
}

using HMSHotel.DTO.Amenity;

namespace HMSHotel.Service.Contracts
{
    public interface IAmenityService
    {
        Task<IEnumerable<AmenityDto>> GetAmenitiesAsync(string? category);
        Task<AmenityDto> GetAmenityByIdAsync(int id);
        Task<AmenityDto> CreateAmenityAsync(CreateAmenityDto dto);
        Task UpdateAmenityAsync(int id, UpdateAmenityDto dto);
        Task DeleteAmenityAsync(int id);
    }
}

using HMSHotel.DTO.HotelAmenity;

namespace HMSHotel.Service.Contracts
{
    public interface IHotelAmenityService
    {
        Task<IEnumerable<HotelAmenityDto>> GetLinksAsync(Guid? hotelId);
        Task<HotelAmenityDto> LinkAsync(HotelAmenityDto dto);
        Task UnlinkAsync(Guid hotelId, int amenityId);
    }
}

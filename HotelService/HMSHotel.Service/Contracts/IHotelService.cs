using HMSHotel.DTO.Hotel;

namespace HMSHotel.Service.Contracts
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelDto>> GetAllHotelsAsync();
        Task<HotelDto> GetHotelByIdAsync(Guid id);
        Task<HotelDto> CreateHotelAsync(CreateHotelDto dto);
        Task UpdateHotelAsync(Guid id, UpdateHotelDto dto);
        Task DeleteHotelAsync(Guid id);
    }
}

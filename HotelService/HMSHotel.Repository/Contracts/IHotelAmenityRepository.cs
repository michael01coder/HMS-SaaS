using HMSHotel.Domain.Data;

namespace HMSHotel.Repository.Contracts
{
    public interface IHotelAmenityRepository
    {
        Task<IEnumerable<HotelAmenity>> GetAllAsync();
        Task<IEnumerable<HotelAmenity>> GetByHotelAsync(Guid hotelId);
        Task<HotelAmenity?> FindAsync(Guid hotelId, int amenityId);
        Task AddAsync(HotelAmenity entity);
        void Remove(HotelAmenity entity);
        Task<bool> SaveChangesAsync();
    }
}

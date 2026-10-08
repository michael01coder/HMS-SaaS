using HMSRoomManagement.Domain.Data;

namespace HMSRoomManagement.Repository.Contracts
{
    public interface IRoomTypeAmenityRepository
    {
        Task<IEnumerable<RoomTypeAmenity>> GetAllAsync();
        Task<IEnumerable<RoomTypeAmenity>> GetByRoomTypeAsync(int roomTypeId);
        Task<RoomTypeAmenity?> FindAsync(int roomTypeId, int amenityId);
        Task AddAsync(RoomTypeAmenity entity);
        void Remove(RoomTypeAmenity entity);
        Task<bool> SaveChangesAsync();
    }
}

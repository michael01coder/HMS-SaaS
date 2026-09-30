using HMSRoomManagement.Data;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.Repository.Contract;
using Microsoft.EntityFrameworkCore;


namespace HMSRoomManagement.Repository.Repository
{
    public class RoomTypeAmenityRepository : IRoomTypeAmenityRepository
    {
        private readonly HMSRoomManagementDbContext _context;
        public RoomTypeAmenityRepository(HMSRoomManagementDbContext context) => _context = context;

        public async Task<IEnumerable<RoomTypeAmenity>> GetAllAsync()
        {
            return await _context.RoomTypeAmenities.ToListAsync();
        }

        public async Task<IEnumerable<RoomTypeAmenity>> GetByRoomTypeAsync(int roomTypeId) =>
            await _context.RoomTypeAmenities.Where(x => x.RoomTypeId == roomTypeId).ToListAsync();

        public async Task<RoomTypeAmenity?> FindAsync(int roomTypeId, int amenityId) =>
            await _context.RoomTypeAmenities.FirstOrDefaultAsync(x => x.RoomTypeId == roomTypeId && x.AmenityId == amenityId);

        public async Task AddAsync(RoomTypeAmenity entity) => 
            await _context.RoomTypeAmenities.AddAsync(entity);

        public void Remove(RoomTypeAmenity entity) => 
            _context.RoomTypeAmenities.Remove(entity);

        public async Task<bool> SaveChangesAsync() => 
            await _context.SaveChangesAsync() > 0;
    }
}

using Microsoft.EntityFrameworkCore;
using HMSRoomManagement.Domain;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.Repository.Contracts;

namespace HMSRoomManagement.Repository
{
    public class RoomTypeAmenityRepository : IRoomTypeAmenityRepository
    {
        private readonly HMSRoomManagementDbContext _context;
        public RoomTypeAmenityRepository(HMSRoomManagementDbContext context) => _context = context;

        public async Task<IEnumerable<RoomTypeAmenity>> GetAllAsync() =>
            await _context.RoomTypeAmenities.AsNoTracking().ToListAsync();

        public async Task<IEnumerable<RoomTypeAmenity>> GetByRoomTypeAsync(int roomTypeId) =>
            await _context.RoomTypeAmenities.AsNoTracking().Where(x => x.RoomTypeId == roomTypeId).ToListAsync();

        public async Task<RoomTypeAmenity?> FindAsync(int roomTypeId, int amenityId) =>
            await _context.RoomTypeAmenities.FirstOrDefaultAsync(x => x.RoomTypeId == roomTypeId && x.AmenityId == amenityId);

        public async Task AddAsync(RoomTypeAmenity entity) => await _context.RoomTypeAmenities.AddAsync(entity);

        public void Remove(RoomTypeAmenity entity) => _context.RoomTypeAmenities.Remove(entity);

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}

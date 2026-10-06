using Microsoft.EntityFrameworkCore;
using HMSHotel.Domain;
using HMSHotel.Domain.Data;
using HMSHotel.Repository.Contracts;

namespace HMSHotel.Repository
{
    public class HotelAmenityRepository : IHotelAmenityRepository
    {
        private readonly HotelServiceDbContext _context;
        public HotelAmenityRepository(HotelServiceDbContext context) => _context = context;

        public async Task<IEnumerable<HotelAmenity>> GetAllAsync() =>
            await _context.HotelAmenities.AsNoTracking().ToListAsync();

        public async Task<IEnumerable<HotelAmenity>> GetByHotelAsync(Guid hotelId) =>
            await _context.HotelAmenities.AsNoTracking().Where(x => x.HotelId == hotelId).ToListAsync();

        public async Task<HotelAmenity?> FindAsync(Guid hotelId, int amenityId) =>
            await _context.HotelAmenities.FirstOrDefaultAsync(x => x.HotelId == hotelId && x.AmenityId == amenityId);

        public async Task AddAsync(HotelAmenity entity) => await _context.HotelAmenities.AddAsync(entity);

        public void Remove(HotelAmenity entity) => _context.HotelAmenities.Remove(entity);

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}

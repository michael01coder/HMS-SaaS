using HMSHotel.Domain.Data;
using Microsoft.EntityFrameworkCore;


namespace HMSHotel.Data
{
    public class HotelServiceDbContext : DbContext
    {
        public HotelServiceDbContext(DbContextOptions<HotelServiceDbContext> options) : base(options) { }

        public DbSet<Hotel> Hotels => Set<Hotel>();

    }
}

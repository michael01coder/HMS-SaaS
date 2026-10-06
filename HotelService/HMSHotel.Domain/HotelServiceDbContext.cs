using HMSHotel.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace HMSHotel.Domain
{
    public class HotelServiceDbContext : DbContext
    {
        public HotelServiceDbContext(DbContextOptions<HotelServiceDbContext> options) : base(options) { }

        public DbSet<Hotel> Hotels => Set<Hotel>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<HotelAmenity> HotelAmenities => Set<HotelAmenity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HotelAmenity>()
                .HasKey(ha => new { ha.HotelId, ha.AmenityId });

            base.OnModelCreating(modelBuilder);
        }
    }
}

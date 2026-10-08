using HMSRoomManagement.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace HMSRoomManagement.Domain
{
    public class HMSRoomManagementDbContext : DbContext
    {
        public HMSRoomManagementDbContext(DbContextOptions<HMSRoomManagementDbContext> options) : base(options) { }

        public DbSet<RoomType> RoomTypes => Set<RoomType>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RatePlan> RatePlans => Set<RatePlan>();
        public DbSet<CancellationPolicy> CancellationPolicies => Set<CancellationPolicy>();
        public DbSet<RoomTypeAmenity> RoomTypeAmenities => Set<RoomTypeAmenity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomTypeAmenity>()
                .HasKey(rta => new { rta.RoomTypeId, rta.AmenityId });

            modelBuilder.Entity<CancellationPolicy>()
                .HasIndex(cp => cp.RatePlanId)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

    }
}

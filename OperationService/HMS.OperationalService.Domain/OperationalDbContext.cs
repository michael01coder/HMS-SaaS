using HMS.OperationalService.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace HMS.OperationalService.Domain
{
    public class OperationalDbContext : DbContext
    {
        public OperationalDbContext(DbContextOptions<OperationalDbContext> options) : base(options) { }

        public DbSet<HouseKeepingTask> HousekeepingTasks => Set<HouseKeepingTask>();
        public DbSet<MaintainanceRequest> MaintenanceRequests => Set<MaintainanceRequest>();
        public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
        public DbSet<RoomInspection> RoomInspections => Set<RoomInspection>();
        public DbSet<LostAndFound> LostAndFounds => Set<LostAndFound>();
        public DbSet<StaffTaskAssignment> StaffTaskAssignments => Set<StaffTaskAssignment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HouseKeepingTask>(e =>
            {
                e.HasKey(x => x.HousekeepingTaskId);
                e.HasIndex(x => new { x.HotelId, x.Status });
                e.HasIndex(x => x.TaskCode).IsUnique();
            });
            modelBuilder.Entity<MaintainanceRequest>(e =>
            {
                e.HasKey(x => x.MaintenanceRequestId);
                e.HasIndex(x => new { x.HotelId, x.Status });
                e.HasIndex(x => x.RequestCode).IsUnique();
            });
            modelBuilder.Entity<ServiceRequest>(e =>
            {
                e.HasKey(x => x.ServiceRequestId);
                e.HasIndex(x => new { x.HotelId, x.Status });
                e.HasIndex(x => x.RequestCode).IsUnique();
            });
            modelBuilder.Entity<RoomInspection>(e =>
            {
                e.HasKey(x => x.InspectionId);
                e.HasIndex(x => x.HotelId);
                e.HasIndex(x => x.InspectionCode).IsUnique();
            });
            modelBuilder.Entity<LostAndFound>(e =>
            {
                e.HasKey(x => x.LostAndFoundId);
                e.HasIndex(x => x.HotelId);
                e.HasIndex(x => x.ItemCode).IsUnique();
            });
            modelBuilder.Entity<StaffTaskAssignment>(e =>
            {
                e.HasKey(x => x.AssignmentId);
                e.HasIndex(x => new { x.HotelId, x.StaffId });
            });
        }
    }
}

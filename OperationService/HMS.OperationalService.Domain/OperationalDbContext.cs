using HMS.OperationalService.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

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

            modelBuilder.Entity<HouseKeepingTask>().HasKey(x => x.HousekeepingTaskId);
            modelBuilder.Entity<MaintainanceRequest>().HasKey(x => x.MaintainanceRequestId);
            modelBuilder.Entity<ServiceRequest>().HasKey(x => x.ServiceRequestId);
            modelBuilder.Entity<RoomInspection>().HasKey(x => x.InspectionId);
            modelBuilder.Entity<LostAndFound>().HasKey(x => x.LostAndFoundId);
            modelBuilder.Entity<StaffTaskAssignment>().HasKey(x => x.AssignmentId);
        }
    }
}

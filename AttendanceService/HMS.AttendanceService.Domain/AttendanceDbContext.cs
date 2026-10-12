using HMS.AttendanceService.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace HMS.AttendanceService.Domain
{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options) { }

        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<AttendanceAnomaly> AttendanceAnomalies { get; set; }
        public DbSet<LeaveApplication> LeaveApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.Property(a => a.ClockInTime)
                      .HasColumnType("timestamp with time zone");

                entity.Property(a => a.ClockOutTime)
                      .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.HotelId, a.StaffId, a.AttendanceDate })
                .IsUnique();

            modelBuilder.Entity<AttendanceAnomaly>()
                .HasOne(a => a.Attendance)
                .WithMany(at => at.Anomalies)
                .HasForeignKey(a => a.AttendanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LeaveApplication>()
                .HasIndex(l => new { l.HotelId, l.StaffId, l.StartDate });

            modelBuilder.Entity<ShiftAllocation>(entity =>
            {
                entity.HasOne(sa => sa.Shift)
                      .WithMany()
                      .HasForeignKey(sa => sa.ShiftId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ShiftSwapRequest>(entity =>
            {
                entity.HasOne(sr => sr.RequesterAllocation)
                      .WithMany()
                      .HasForeignKey(sr => sr.RequesterAllocationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sr => sr.TargetAllocation)
                      .WithMany()
                      .HasForeignKey(sr => sr.TargetAllocationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
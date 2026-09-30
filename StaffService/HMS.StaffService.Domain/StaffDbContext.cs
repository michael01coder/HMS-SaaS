using HMS.StaffService.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HMS.StaffService.Domain
{
    public class StaffDbContext : DbContext
    {
        public StaffDbContext(DbContextOptions<StaffDbContext> options) : base(options) { }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<StaffDocument> StaffDocuments { get; set; }
        public DbSet<StaffNotes> StaffNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Department)
                .WithMany(d => d.StaffMembers)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>().ToTable("Department");

            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Manager)
                .WithMany(s => s.Subordinates)
                .HasForeignKey(s => s.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StaffDocument>()
                .HasOne(d => d.Staff)
                .WithMany(s => s.Documents)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StaffNotes>()
                .HasOne(n => n.Staff)
                .WithMany(s => s.Notes)
                .HasForeignKey(n => n.StaffId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
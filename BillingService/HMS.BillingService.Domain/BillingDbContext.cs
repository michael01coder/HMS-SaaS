using HMS.BillingService.Domain.Data;
using Microsoft.EntityFrameworkCore;


namespace HMS.BillingService.Domain
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                entity.HasIndex(e => new { e.HotelId, e.BookingId });
                entity.HasMany(i => i.Payments)
                      .WithOne(p => p.Invoice)
                      .HasForeignKey(p => p.InvoiceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasIndex(p => p.TransactionId).IsUnique();
                entity.HasIndex(p => p.HotelId);
            });
        }
    }
}

using HMS.GuestService.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HMS.GuestService.Domain;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<Guest> Guests { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(b => b.BookingReference).IsUnique();
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasIndex(g => g.Email).IsUnique();
        });
    }
}
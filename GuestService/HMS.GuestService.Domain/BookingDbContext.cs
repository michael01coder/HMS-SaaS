using HMS.GuestService.Domain.Data;
using Microsoft.EntityFrameworkCore;

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
            entity.HasIndex(b => new { b.HotelId, b.RoomId, b.CheckInDate });
            entity.HasIndex(b => new { b.HotelId, b.GuestId });
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasIndex(g => new { g.HotelId, g.Email }).IsUnique();
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasIndex(r => r.ReservationCode).IsUnique();
            entity.HasIndex(r => new { r.HotelId, r.BookingId });
        });
    }
}

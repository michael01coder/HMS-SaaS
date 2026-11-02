using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.GuestService.Domain.Data;

[Table("Reservations")]
public class Reservation
{
    [Key]
    public int ReservationId { get; set; }

    [Required]
    public Guid HotelId { get; set; }

    [Required]
    public int BookingId { get; set; } 

    [Required]
    public Guid GuestId { get; set; }   

    [Required]
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(50)]
    public string ReservationStatus { get; set; } = "Active";

    [Required]
    [MaxLength(20)]
    public string ReservationCode { get; set; } = string.Empty;

    [MaxLength(500)]
    public string SpecialRequests { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMS.GuestService.Domain.Data
{
    [Table("Bookings")]
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public string BookingReference { get; set; } = string.Empty;

        [Required]
        public Guid GuestId { get; set; } 

        [Required]
        public int RoomId { get; set; }  

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // e.g., Pending, Confirmed, Cancelled, Completed

        
        [MaxLength(50)]
        public string? PaymentStatus { get; set; } = "Partially"; // e.g., Partially, Paid, Not Paid

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}

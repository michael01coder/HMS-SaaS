using System.ComponentModel.DataAnnotations;

namespace HMS.GuestService.DTO.BookingDTO
{
    public class UpdateBookingDTO
    {
        [Required]
        public string Status { get; set; } = string.Empty;

        public string? PaymentStatus { get; set; }
    }
}

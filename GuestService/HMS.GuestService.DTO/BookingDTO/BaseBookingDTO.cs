using System.ComponentModel.DataAnnotations;

namespace HMS.GuestService.DTO.BookingDTO
{
    public class BaseBookingDTO
    {
        [Required]
        public Guid GuestId { get; set; }

        [Range(1, int.MaxValue)]
        public int RoomId { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";
        public string PaymentStatus { get; set; } = "Pending";
    }
}

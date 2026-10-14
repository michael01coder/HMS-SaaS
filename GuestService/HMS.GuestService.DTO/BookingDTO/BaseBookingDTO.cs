using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.DTO.BookingDTO
{
    public class BaseBookingDTO
    {
        public Guid GuestId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public string PaymentStatus { get; set; } = "Partially";
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.DTO.BookingDTO
{
    public class BookingDTO : BaseBookingDTO
    {
        public int BookingId { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

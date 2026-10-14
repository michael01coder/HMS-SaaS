using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.DTO.ReservationDTO
{
    public class BaseReservationDTO
    {
        public int BookingId { get; set; }
        public Guid GuestId { get; set; }
        public string ReservationCode { get; set; } = string.Empty;

        public string ReservationStatus { get; set; } = "Active";
        public string SpecialRequests { get; set; } = string.Empty;
    }
}

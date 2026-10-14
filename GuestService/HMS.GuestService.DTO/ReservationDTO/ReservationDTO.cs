using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.DTO.ReservationDTO
{
    public class ReservationDTO : BaseReservationDTO
    {
        public int ReservationId { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

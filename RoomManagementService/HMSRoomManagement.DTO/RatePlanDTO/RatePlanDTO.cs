using System;
using System.Collections.Generic;
using System.Text;

namespace HMSRoomManagement.DTO.RatePlanDTO
{

    public class RatePlanDTO
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public bool Refundable { get; set; }
        public bool BreakfastIncluded { get; set; }
        public bool PayAtHotel { get; set; }
        public bool IsActive { get; set; }

        //one row per rate plan
        public int FreeCancellationUntilHours { get; set; }
        public decimal PenaltyPercentage { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace HMSRoomManagement.DTO.RatePlanDTO
{
    public class UpdateRatePlanDTO
    {
        public string PlanName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public bool Refundable { get; set; }
        public bool BreakfastIncluded { get; set; }
        public bool PayAtHotel { get; set; }
        public bool IsActive { get; set; }
        public int FreeCancellationUntilHours { get; set; }
        public decimal PenaltyPercentage { get; set; }
    }
}

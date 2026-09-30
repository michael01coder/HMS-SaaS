using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMSRoomManagement.DTO.RatePlanDTO
{
    public class CreateRatePlanDTO
    {
        [Required] public int RoomTypeId { get; set; }
        [Required] public string PlanName { get; set; } = string.Empty;
        [Required] public decimal PricePerNight { get; set; }
        public bool Refundable { get; set; }
        public bool BreakfastIncluded { get; set; }
        public bool PayAtHotel { get; set; }
        public bool IsActive { get; set; } = true;
        [Required] public int FreeCancellationUntilHours { get; set; }
        [Required] public decimal PenaltyPercentage { get; set; }
    }
}

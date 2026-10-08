using System.ComponentModel.DataAnnotations;

namespace HMSRoomManagement.DTO.RatePlan
{
    public class CreateRatePlanDto
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

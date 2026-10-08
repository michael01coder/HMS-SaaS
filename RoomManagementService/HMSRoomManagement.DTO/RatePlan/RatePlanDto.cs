namespace HMSRoomManagement.DTO.RatePlan
{
    // Flattened — RatePlans and CancellationPolicies are two real tables
    // (1:1), composed into one DTO here by the Service layer, same as your
    // original UI spec asked for.
    public class RatePlanDto
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
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

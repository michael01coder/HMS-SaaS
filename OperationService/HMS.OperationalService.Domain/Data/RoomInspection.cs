using HMS.OperationalService.Common.Enums;

namespace HMS.OperationalService.Domain.Data
{
    public class RoomInspection
    {
        public int InspectionId { get; set; }
        public Guid HotelId { get; set; }
        public string InspectionCode { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public Guid InspectorId { get; set; }
        public InspectionTyperoutine InspectionType { get; set; }
        public int ConditionRating { get; set; }
        public string? Remarks { get; set; }
        public DateTime InspectionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

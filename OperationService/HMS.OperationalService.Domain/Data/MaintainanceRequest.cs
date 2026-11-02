using HMS.OperationalService.Common.Enums;

namespace HMS.OperationalService.Domain.Data
{
    public class MaintainanceRequest
    {
        public int MaintenanceRequestId { get; set; }
        public Guid HotelId { get; set; }
        public string RequestCode { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public MaintenanceCategory Category { get; set; }
        public string IssueTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; }
        public Common.Enums.TaskStatus Status { get; set; }
        public Guid? AssignedTechnicianId { get; set; }
        public Guid? ReportedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

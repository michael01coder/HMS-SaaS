using HMS.OperationalService.Common.Enums;

namespace HMS.OperationalService.Domain.Data
{
    public class StaffTaskAssignment
    {
        public int AssignmentId { get; set; }
        public Guid HotelId { get; set; }
        public Guid StaffId { get; set; }
        public OperationalTaskType TaskType { get; set; }
        public int TaskReferenceId { get; set; }
        public Guid AssignedBy { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public Common.Enums.TaskStatus Status { get; set; }
        public string? Remarks { get; set; }
    }
}

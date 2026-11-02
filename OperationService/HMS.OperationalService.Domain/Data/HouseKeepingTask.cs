using HMS.OperationalService.Common.Enums;

namespace HMS.OperationalService.Domain.Data
{
    public class HouseKeepingTask
    {
        public int HousekeepingTaskId { get; set; }
        public Guid HotelId { get; set; }
        public string TaskCode { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public HousekeepingTaskType TaskType { get; set; }
        public Guid AssignedStaffId { get; set; }
        public TaskPriority Priority { get; set; }
        public Common.Enums.TaskStatus Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

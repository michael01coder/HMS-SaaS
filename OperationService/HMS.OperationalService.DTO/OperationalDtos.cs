using System.ComponentModel.DataAnnotations;
using HMS.OperationalService.Common.Enums;

namespace HMS.OperationalService.DTO
{
    public class HousekeepingTaskDto
    {
        [Range(1, int.MaxValue)]
        public int RoomId { get; set; }
        public HousekeepingTaskType TaskType { get; set; }
        public Guid AssignedStaffId { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public Common.Enums.TaskStatus Status { get; set; } = Common.Enums.TaskStatus.Pending;
        [MaxLength(500)]
        public string? Remarks { get; set; }
        public DateTime ScheduledDate { get; set; }
    }

    public class MaintenanceRequestDto
    {
        [Range(1, int.MaxValue)]
        public int RoomId { get; set; }
        public MaintenanceCategory Category { get; set; }
        [Required, MaxLength(150)]
        public string IssueTitle { get; set; } = string.Empty;
        [Required, MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public Common.Enums.TaskStatus Status { get; set; } = Common.Enums.TaskStatus.Pending;
        public Guid? AssignedTechnicianId { get; set; }
        public Guid? ReportedBy { get; set; }
    }

    public class ServiceRequestDto
    {
        public Guid GuestId { get; set; }
        [Range(1, int.MaxValue)]
        public int RoomId { get; set; }
        public ServiceRequestType RequestType { get; set; }
        [Required, MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        public Guid? AssignedStaffId { get; set; }
        public Common.Enums.TaskStatus Status { get; set; } = Common.Enums.TaskStatus.Pending;
    }

    public class RoomInspectionDto
    {
        [Range(1, int.MaxValue)]
        public int RoomId { get; set; }
        public Guid InspectorId { get; set; }
        public InspectionTyperoutine InspectionType { get; set; }
        [Range(1, 5)]
        public int ConditionRating { get; set; }
        [MaxLength(500)]
        public string? Remarks { get; set; }
    }

    public class LostAndFoundDto
    {
        [Range(1, int.MaxValue)]
        public int RoomId { get; set; }
        [Required, MaxLength(150)]
        public string ItemName { get; set; } = string.Empty;
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        public Guid FoundByStaffId { get; set; }
        [Required, MaxLength(150)]
        public string StorageLocation { get; set; } = string.Empty;
    }

    public class StaffTaskAssignmentDto
    {
        public Guid StaffId { get; set; }
        public OperationalTaskType TaskType { get; set; }
        [Range(1, int.MaxValue)]
        public int TaskReferenceId { get; set; }
        public Guid AssignedBy { get; set; }
        public Common.Enums.TaskStatus Status { get; set; } = Common.Enums.TaskStatus.Pending;
        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}

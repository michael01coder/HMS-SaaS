using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.DTO.HouseKeepingDTO
{
    public class HouseKeepingTaskResponseDTO
    {
        public int HousekeepingTaskId { get; set; }
        public string TaskCode { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public Guid AssignedStaffId { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.DTO.HouseKeepingDTO
{
    public class CreateHouseKeepingTaskDTO
    {
        public int RoomId { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public Guid AssignedStaffId { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime ScheduledDate { get; set; }
    }
}

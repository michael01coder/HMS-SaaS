using HMS.OperationalService.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Domain.Data
{
    public class StaffTaskAssignment
    {
        public int AssignmentId { get; set; }
        public Guid StaffId { get; set; }
        public OperationalTaskType TaskType { get; set; }
        public int TaskReferenceId { get; set; }
        public Guid AssignedBy { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public HMS.OperationalService.Common.Enums.TaskStatus Status { get; set; }
        public string? Remarks { get; set; }
    }
}

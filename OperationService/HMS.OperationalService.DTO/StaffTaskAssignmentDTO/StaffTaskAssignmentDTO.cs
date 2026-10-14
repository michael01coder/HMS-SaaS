using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.DTO.StaffTaskAssignmentDTO
{
    public class StaffTaskAssignmentDTO
    {
        public int AssignmentId { get; set; }
        public Guid StaffId { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public int TaskReferenceId { get; set; }
        public Guid AssignedBy { get; set; }
        public DateTime AssignedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.DTO.StaffTaskAssignmentDTO
{
    public class CreateStaffTaskAssignmentDTO
    {
        public Guid StaffId { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public int TaskReferenceId { get; set; }
        public Guid AssignedBy { get; set; }
        public string? Remarks { get; set; }
    }
}

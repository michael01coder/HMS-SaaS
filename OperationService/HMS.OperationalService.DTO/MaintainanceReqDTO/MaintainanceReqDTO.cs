using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.DTO.MaintainanceReqDTO
{
    public class MaintainanceReqDTO
    {
        public int MaintenanceRequestId { get; set; }
        public string RequestCode { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string IssueTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid? AssignedTechnicianId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}

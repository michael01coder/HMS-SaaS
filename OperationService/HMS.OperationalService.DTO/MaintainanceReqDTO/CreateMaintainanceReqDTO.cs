using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.DTO.MaintainanceReqDTO
{
    public class CreateMaintainanceReqDTO
    {
        public int RoomId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string IssueTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public Guid? ReportedBy { get; set; }
    }
}

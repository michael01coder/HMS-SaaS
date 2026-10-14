using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.DTO.ServiceReqDTO
{
    public class ServiceReqDTO
    {
        public int ServiceRequestId { get; set; }
        public string RequestCode { get; set; } = string.Empty;
        public Guid GuestId { get; set; }
        public int RoomId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? AssignedStaffId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

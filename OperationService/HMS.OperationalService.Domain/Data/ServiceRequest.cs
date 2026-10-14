using HMS.OperationalService.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Domain.Data
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }
        public string RequestCode { get; set; } = string.Empty;
        public Guid GuestId { get; set; }
        public int RoomId { get; set; }
        public ServiceRequestType RequestType { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid? AssignedStaffId { get; set; }
        public HMS.OperationalService.Common.Enums.TaskStatus Status { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

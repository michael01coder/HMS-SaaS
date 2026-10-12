using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.AttendanceService.DTO.Shift
{
    public class ShiftSwapRequestDto
    {
        public Guid SwapRequestId { get; set; }
        public Guid TenantId { get; set; }
        public Guid HotelId { get; set; }
        public Guid RequesterStaffId { get; set; }
        public Guid RequesterAllocationId { get; set; }
        public Guid TargetStaffId { get; set; }
        public Guid? TargetAllocationId { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "Pending";
        public Guid? ApprovedByStaffId { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public ShiftAllocationDto? RequesterAllocation { get; set; }
        public ShiftAllocationDto? TargetAllocation { get; set; }
    }
}

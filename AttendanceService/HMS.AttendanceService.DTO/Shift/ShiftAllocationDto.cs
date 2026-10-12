using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.AttendanceService.DTO.Shift
{
    public class ShiftAllocationDto
    {
        public Guid AllocationId { get; set; }
        public Guid TenantId { get; set; }
        public Guid HotelId { get; set; }
        public Guid StaffId { get; set; }
        public Guid ShiftId { get; set; }
        public DateOnly WorkDate { get; set; }
        public string AllocationType { get; set; } = "Regular";
        public DateTime CreatedAt { get; set; }
        public ShiftDto? Shift { get; set; }
    }
}

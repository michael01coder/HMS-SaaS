using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.AttendanceService.DTO.Shift
{
    public class CreateShiftSwapRequestDto
    {
        [Required]
        public Guid RequesterStaffId { get; set; }

        [Required]
        public Guid RequesterAllocationId { get; set; }

        [Required]
        public Guid TargetStaffId { get; set; }

        public Guid? TargetAllocationId { get; set; }

        [MaxLength(255)]
        public string? Reason { get; set; }
    }
}

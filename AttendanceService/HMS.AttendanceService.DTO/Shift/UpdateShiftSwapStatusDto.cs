using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.AttendanceService.DTO.Shift
{
    public class UpdateShiftSwapStatusDto
    {
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        public Guid? ApprovedByStaffId { get; set; }

        public Guid? TargetStaffId { get; set; }

        public Guid? TargetAllocationId { get; set; }
    }
}

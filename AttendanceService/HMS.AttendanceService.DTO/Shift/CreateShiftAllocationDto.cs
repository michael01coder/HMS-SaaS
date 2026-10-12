using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMS.AttendanceService.DTO.Shift
{
    public class CreateShiftAllocationDto
    {
        [Required]
        public Guid StaffId { get; set; }

        [Required]
        public Guid ShiftId { get; set; }

        [Required]
        public DateOnly WorkDate { get; set; }

        [MaxLength(20)]
        public string AllocationType { get; set; } = "Regular";
    }
}

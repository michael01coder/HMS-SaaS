using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.AttendanceService.DTO.Shift
{
    public class UpdateShiftDto
    {
        public string? ShiftName { get; set; }
        public string? ShiftCategory { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool? IsActive { get; set; }
    }
}

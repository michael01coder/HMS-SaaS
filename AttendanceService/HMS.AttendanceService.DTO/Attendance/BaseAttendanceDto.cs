using System.ComponentModel.DataAnnotations;

namespace HMS.AttendanceService.DTO.Attendance
{
    public class BaseAttendanceDto
    {
        [Required]
        public Guid StaffId { get; set; }

        [Required]
        public DateOnly AttendanceDate { get; set; }

        public DateTime ClockInTime { get; set; }

        public DateTime? ClockOutTime { get; set; }

        public decimal TotalHours { get; set; } = 0;

        public decimal OvertimeHours { get; set; } = 0;

        [MaxLength(20)]
        public string Status { get; set; } = "Present";

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}
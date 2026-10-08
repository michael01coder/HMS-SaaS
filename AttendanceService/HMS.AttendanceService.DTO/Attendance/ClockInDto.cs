using System.ComponentModel.DataAnnotations;

namespace HMS.AttendanceService.DTO.Attendance
{
    public class ClockInDto
    {
        [Required]
        public Guid StaffId { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}
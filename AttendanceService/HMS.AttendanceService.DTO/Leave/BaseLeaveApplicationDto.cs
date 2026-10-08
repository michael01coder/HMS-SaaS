using System.ComponentModel.DataAnnotations;

namespace HMS.AttendanceService.DTO.Leave
{
    public class BaseLeaveApplicationDto
    {
        [Required]
        public Guid StaffId { get; set; }

        [Required]
        [MaxLength(50)]
        public string LeaveTypeName { get; set; } = string.Empty;

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        public int TotalDays { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        public int DefaultDaysPerYear { get; set; } = 12;
    }
}
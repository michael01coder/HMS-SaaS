using System.ComponentModel.DataAnnotations;

namespace HMS.AttendanceService.DTO.Shift
{
    public class CreateShiftDto
    {
        [Required]
        [MaxLength(50)]
        public string ShiftName { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string ShiftCategory { get; set; } = string.Empty;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}
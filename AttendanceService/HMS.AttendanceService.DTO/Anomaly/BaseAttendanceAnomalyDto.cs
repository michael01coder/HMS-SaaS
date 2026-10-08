using System.ComponentModel.DataAnnotations;

namespace HMS.AttendanceService.DTO.Anomaly
{
    public class BaseAttendanceAnomalyDto
    {
        [Required]
        public Guid AttendanceId { get; set; }

        [Required]
        public Guid StaffId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AnomalyType { get; set; } = string.Empty; // LateArrival, EarlyDeparture, MissingClockOut

        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Resolved, Ignored
    }
}
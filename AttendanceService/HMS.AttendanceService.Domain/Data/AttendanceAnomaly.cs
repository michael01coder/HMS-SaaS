using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.AttendanceService.Domain.Data
{
    [Table("AttendanceAnomalies")]
    public class AttendanceAnomaly
    {
        [Key]
        public Guid AnomalyId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid AttendanceId { get; set; }

        [Required]
        public Guid StaffId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AnomalyType { get; set; } = string.Empty; // LateArrival, EarlyDeparture, MissingClockOut

        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Resolved, Ignored

        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(AttendanceId))]
        public virtual Attendance? Attendance { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.AttendanceService.Domain.Data
{
    [Table("Attendances")]
    public class Attendance
    {
        [Key]
        public Guid AttendanceId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid HotelId { get; set; }

        [Required]
        public Guid StaffId { get; set; }

        [Required]
        public DateOnly AttendanceDate { get; set; }

        public DateTime ClockInTime { get; set; }

        public DateTime? ClockOutTime { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal TotalHours { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal OvertimeHours { get; set; } = 0;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Present";

        [MaxLength(500)]
        public string? Remarks { get; set; }

        public bool IsApproved { get; set; } = false;

        public Guid? ApprovedByStaffId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<AttendanceAnomaly> Anomalies { get; set; } = new List<AttendanceAnomaly>();
    }
}
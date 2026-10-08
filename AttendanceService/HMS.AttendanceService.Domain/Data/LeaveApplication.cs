using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.AttendanceService.Domain.Data
{
    [Table("LeaveApplications")]
    public class LeaveApplication
    {
        [Key]
        public Guid LeaveApplicationId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid HotelId { get; set; }

        [Required]
        public Guid StaffId { get; set; }

        [Required]
        [MaxLength(50)]
        public string LeaveTypeName { get; set; } = string.Empty; // e.g., Sick Leave, Casual Leave

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        public int TotalDays { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Cancelled

        public DateOnly AppliedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public Guid? ApprovedByStaffId { get; set; }

        public DateOnly? ApprovalDate { get; set; }

        public int DefaultDaysPerYear { get; set; } = 12;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
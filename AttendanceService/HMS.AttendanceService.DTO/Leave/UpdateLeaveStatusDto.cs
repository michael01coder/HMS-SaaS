using System.ComponentModel.DataAnnotations;

namespace HMS.AttendanceService.DTO.Leave
{
    public class UpdateLeaveStatusDto
    {
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty; // Approved, Rejected, Cancelled

        public Guid? ApprovedByStaffId { get; set; }
    }
}
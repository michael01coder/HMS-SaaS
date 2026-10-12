using HMS.AttendanceService.Domain.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.AttendanceService.Domain.Data
{
    [Table("ShiftSwapRequests")]
    public class ShiftSwapRequest
    {
        [Key]
        public Guid SwapRequestId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid HotelId { get; set; }

        [Required]
        public Guid RequesterStaffId { get; set; }

        [Required]
        public Guid RequesterAllocationId { get; set; }

        [Required]
        public Guid TargetStaffId { get; set; }

        public Guid? TargetAllocationId { get; set; } // Optional: exchange shift for another shift

        [MaxLength(255)]
        public string? Reason { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Cancelled

        public Guid? ApprovedByStaffId { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RespondedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(RequesterAllocationId))]
        public virtual ShiftAllocation? RequesterAllocation { get; set; }

        [ForeignKey(nameof(TargetAllocationId))]
        public virtual ShiftAllocation? TargetAllocation { get; set; }
    }
}
using HMS.AttendanceService.Domain.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.AttendanceService.Domain.Data
{
    [Table("ShiftAllocations")]
    public class ShiftAllocation
    {
        [Key]
        public Guid AllocationId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid HotelId { get; set; }

        [Required]
        public Guid StaffId { get; set; }

        [Required]
        public Guid ShiftId { get; set; }

        public DateOnly WorkDate { get; set; } // Specific date assigned

        [MaxLength(20)]
        public string AllocationType { get; set; } = "Regular"; // Regular, Recurring, Overtime

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ShiftId))]
        public virtual Shift? Shift { get; set; }
    }
}
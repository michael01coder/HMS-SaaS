using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.AttendanceService.Domain.Data
{
    [Table("Shifts")]
    public class Shift
    {
        [Key]
        public Guid ShiftId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid HotelId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ShiftName { get; set; } = string.Empty; // e.g., Morning, Night, General

        [Required]
        [MaxLength(30)]
        public string ShiftCategory { get; set; } = string.Empty; // e.g., Regular, Overtime, Split

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
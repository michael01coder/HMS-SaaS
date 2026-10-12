using System.ComponentModel.DataAnnotations;

namespace HMS.AttendanceService.DTO.Shift
{
    public class BulkShiftAllocationDto
    {
        [Required]
        public List<Guid> StaffIds { get; set; } = new();

        [Required]
        public Guid ShiftId { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [MaxLength(20)]
        public string AllocationType { get; set; } = "Regular";
    }
}
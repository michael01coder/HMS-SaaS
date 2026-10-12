using System.ComponentModel.DataAnnotations;

namespace HMS.AttendanceService.DTO.Shift
{
    public class ShiftDto
    {
        public Guid ShiftId { get; set; }
        public Guid TenantId { get; set; }
        public Guid HotelId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string ShiftCategory { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
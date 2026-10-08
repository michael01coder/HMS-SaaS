namespace HMS.AttendanceService.DTO.Attendance
{
    public class AttendanceDto : BaseAttendanceDto
    {
        public Guid AttendanceId { get; set; }
        public Guid TenantId { get; set; }
        public Guid HotelId { get; set; }
        public bool IsApproved { get; set; }
        public Guid? ApprovedByStaffId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
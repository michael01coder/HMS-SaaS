namespace HMS.AttendanceService.DTO.Attendance
{
    public class CreateAttendanceDto : BaseAttendanceDto
    {
        public Guid TenantId { get; set; }
        public Guid HotelId { get; set; }
    }
}
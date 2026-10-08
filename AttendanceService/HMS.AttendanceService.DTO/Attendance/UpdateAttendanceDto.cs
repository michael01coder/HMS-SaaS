namespace HMS.AttendanceService.DTO.Attendance
{
    public class UpdateAttendanceDto
    {
        public DateTime? ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }
        public decimal? TotalHours { get; set; }
        public decimal? OvertimeHours { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public bool? IsApproved { get; set; }
        public Guid? ApprovedByStaffId { get; set; }
    }
}
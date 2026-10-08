namespace HMS.AttendanceService.DTO.Leave
{
    public class LeaveBalanceDto
    {
        public Guid StaffId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int TotalQuota { get; set; }
        public int UsedDays { get; set; }
        public int RemainingDays { get; set; }
    }
}
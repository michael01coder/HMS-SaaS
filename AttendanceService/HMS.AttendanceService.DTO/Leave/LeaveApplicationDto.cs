namespace HMS.AttendanceService.DTO.Leave
{
    public class LeaveApplicationDto : BaseLeaveApplicationDto
    {
        public Guid LeaveApplicationId { get; set; }
        public Guid TenantId { get; set; }
        public Guid HotelId { get; set; }
        public string Status { get; set; } = "Pending";
        public DateOnly AppliedOn { get; set; }
        public Guid? ApprovedByStaffId { get; set; }
        public DateOnly? ApprovalDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
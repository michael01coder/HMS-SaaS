namespace HMS.StaffService.DTO.Staff
{
    public class StaffDto : BaseStaffDto
    {
        public Guid StaffId { get; set; }
        public Guid HotelId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DepartmentName { get; set; }
        public string? ManagerName { get; set; }
    }
}

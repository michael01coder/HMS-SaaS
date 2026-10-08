namespace HMS.StaffService.DTO.Department
{
    public class DepartmentDto : BaseDepartmentDto
    {
        public int DepartmentId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

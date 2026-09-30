namespace HMS.StaffService.DTO.Staff
{
    public class UpdateStaffDto
    {
        public string? EmployeeCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? DepartmentId { get; set; }
        public string? Designation { get; set; }
        public string? EmploymentType { get; set; }
        public Guid? ManagerId { get; set; }
        public DateOnly? JoiningDate { get; set; }
        public decimal? Salary { get; set; }
        public bool? IsActive { get; set; }
    }
}
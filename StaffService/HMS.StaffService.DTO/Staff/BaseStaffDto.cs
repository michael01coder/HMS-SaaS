using System.ComponentModel.DataAnnotations;

namespace HMS.StaffService.DTO.Staff
{
    public class BaseStaffDto
    {
        [Required]
        [MaxLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string Designation { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = "Full-Time";
        public Guid? ManagerId { get; set; }
        public DateOnly JoiningDate { get; set; }
        public decimal Salary { get; set; }
        public string Role { get; set; } = "Staff";
    }
}
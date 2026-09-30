using System.ComponentModel.DataAnnotations;

namespace HMS.StaffService.DTO.StaffDocument
{
    public class BaseStaffDocumentDto
    {
        [Required]
        public Guid HotelId { get; set; }

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
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Designation { get; set; } = string.Empty;

        [MaxLength(30)]
        public string EmploymentType { get; set; } = "Full-Time";

        public Guid? ManagerId { get; set; }

        public DateTime JoiningDate { get; set; }

        [Range(0, 9999999999999999.99)]
        public decimal Salary { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? ProbationEndDate { get; set; }

        [MaxLength(255)]
        public string? PhotoUrl { get; set; }
    }
}
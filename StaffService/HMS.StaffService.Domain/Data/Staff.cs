using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.StaffService.Domain.Data
{
    [Table("Staff")]
    public class Staff
    {
        [Key]
        public Guid StaffId { get; set; } = Guid.NewGuid();

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

        public string Role { get; set; } = "Staff";

        [Required]
        [MaxLength(30)]
        public string EmploymentType { get; set; } = "Full-Time";

        public Guid? ManagerId { get; set; }

        public DateOnly JoiningDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProbationEndDate { get; set; }

        [MaxLength(255)]
        public string? PhotoUrl { get; set; }

        // Navigation Properties within Staff Domain
        [ForeignKey(nameof(DepartmentId))]
        public virtual Department? Department { get; set; }

        [ForeignKey(nameof(ManagerId))]
        public virtual Staff? Manager { get; set; }

        public virtual ICollection<Staff> Subordinates { get; set; } = new List<Staff>();
        public virtual ICollection<StaffDocument> Documents { get; set; } = new List<StaffDocument>();
        public virtual ICollection<StaffNotes> Notes { get; set; } = new List<StaffNotes>();
    }
}
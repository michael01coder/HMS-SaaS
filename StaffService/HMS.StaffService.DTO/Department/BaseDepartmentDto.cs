using System.ComponentModel.DataAnnotations;

namespace HMS.StaffService.DTO.Department
{
    public class BaseDepartmentDto
    {
        [Required]
        [MaxLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string DepartmentCode { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public int BudgetedHeadcount { get; set; }
    }
}
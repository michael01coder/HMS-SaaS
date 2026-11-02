using System.ComponentModel.DataAnnotations;

namespace HMS.StaffService.DTO.Department
{
    public class BaseDesignationDto
    {
        [Required]
        [MaxLength(50)]
        public string DesignationName { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateDesignationDto : BaseDesignationDto
    {
        public Guid HotelId { get; set; }
    }

    public class DesignationDto : BaseDesignationDto
    {
        public int DesignationId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

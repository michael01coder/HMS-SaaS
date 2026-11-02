using System.ComponentModel.DataAnnotations;

namespace HMS.StaffService.Domain.Data
{
    public class Designation
    {
        [Key]
        public int DesignationId { get; set; }

        [Required]
        public Guid HotelId { get; set; }

        [Required]
        [MaxLength(50)]
        public string DesignationName { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

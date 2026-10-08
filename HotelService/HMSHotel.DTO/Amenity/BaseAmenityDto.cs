using System.ComponentModel.DataAnnotations;

namespace HMSHotel.DTO.Amenity
{
    public class BaseAmenityDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Category { get; set; } = string.Empty; // "Property" or "Room"

        [StringLength(50)]
        public string? Icon { get; set; }
    }
}

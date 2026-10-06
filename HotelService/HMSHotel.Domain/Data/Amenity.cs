using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMSHotel.Domain.Data
{
    [Table("Amenities")]
    public class Amenity
    {
        [Key]
        public int Id { get; set; }

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

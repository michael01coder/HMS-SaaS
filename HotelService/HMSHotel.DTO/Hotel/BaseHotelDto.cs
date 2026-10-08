using System.ComponentModel.DataAnnotations;

namespace HMSHotel.DTO.Hotel
{
    public class BaseHotelDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string PropertyType { get; set; } = string.Empty;

        [Required]
        public int StarRating { get; set; }

        [Required]
        [StringLength(200)]
        public string AddressLine1 { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int TotalFloors { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}

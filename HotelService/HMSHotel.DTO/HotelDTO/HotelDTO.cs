using System.ComponentModel.DataAnnotations;

namespace HMSHotel.DTO.HotelDTO

{
    public class HotelDTO : BaseHotelDTO
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

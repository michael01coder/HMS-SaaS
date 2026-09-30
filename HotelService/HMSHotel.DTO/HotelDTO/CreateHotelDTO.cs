using System.ComponentModel.DataAnnotations;

namespace HMSHotel.DTO.HotelDTO

{
    public class CreateHotelDTO : BaseHotelDTO
    {
        [Required]
        public Guid TenantId { get; set; }
    }
}

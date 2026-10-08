using System.ComponentModel.DataAnnotations;

namespace HMSHotel.DTO.Hotel
{
    public class CreateHotelDto : BaseHotelDto
    {
        // Which tenant this hotel belongs to — required on create, never
        // changeable afterwards (excluded from UpdateHotelDto on purpose).
        [Required]
        public Guid TenantId { get; set; }
    }
}

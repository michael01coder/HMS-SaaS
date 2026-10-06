namespace HMSHotel.DTO.Hotel
{
    public class HotelDto : BaseHotelDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

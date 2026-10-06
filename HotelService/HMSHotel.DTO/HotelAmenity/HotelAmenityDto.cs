namespace HMSHotel.DTO.HotelAmenity
{
    // Link-only — no Create/Update split needed, this IS the create payload,
    // and there is no edit for a junction row, only POST (link) / DELETE (unlink).
    public class HotelAmenityDto
    {
        public Guid HotelId { get; set; }
        public int AmenityId { get; set; }
    }
}

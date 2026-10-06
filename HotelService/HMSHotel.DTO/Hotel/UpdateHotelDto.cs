namespace HMSHotel.DTO.Hotel
{
    public class UpdateHotelDto : BaseHotelDto
    {
        // TenantId intentionally excluded — a hotel's owning tenant doesn't
        // change via a normal edit.
    }
}

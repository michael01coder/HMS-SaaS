namespace HMSRoomManagement.DTO.RoomTypeAmenity
{
    // Link-only — no Create/Update split, this IS the link payload.
    public class RoomTypeAmenityDto
    {
        public int RoomTypeId { get; set; }
        public int AmenityId { get; set; }
    }
}

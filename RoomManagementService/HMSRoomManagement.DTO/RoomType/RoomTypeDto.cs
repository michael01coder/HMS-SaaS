namespace HMSRoomManagement.DTO.RoomType
{
    public class RoomTypeDto : BaseRoomTypeDto
    {
        public int Id { get; set; }
        public Guid HotelId { get; set; }
    }
}

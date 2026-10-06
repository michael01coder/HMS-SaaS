namespace HMSRoomManagement.DTO.Room
{
    public class RoomDto : BaseRoomDto
    {
        public int Id { get; set; }
        public Guid HotelId { get; set; }
        public bool IsActive { get; set; }
    }
}

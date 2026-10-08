using System.ComponentModel.DataAnnotations;

namespace HMSRoomManagement.DTO.RoomType
{
    public class CreateRoomTypeDto : BaseRoomTypeDto
    {
        [Required]
        public Guid HotelId { get; set; }
    }
}

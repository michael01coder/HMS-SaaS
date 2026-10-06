using System.ComponentModel.DataAnnotations;

namespace HMSRoomManagement.DTO.Room
{
    public class CreateRoomDto : BaseRoomDto
    {
        [Required]
        public Guid HotelId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

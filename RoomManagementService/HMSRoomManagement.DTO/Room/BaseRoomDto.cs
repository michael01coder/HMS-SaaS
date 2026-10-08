using System.ComponentModel.DataAnnotations;

namespace HMSRoomManagement.DTO.Room
{
    public class BaseRoomDto
    {
        [Required]
        public int RoomTypeId { get; set; }
        [Required]
        public string RoomNumber { get; set; } = string.Empty;
        [Required]
        public int FloorNumber { get; set; }
        [Required]
        public string Status { get; set; } = "Available";
    }
}

using HMSRoomManagement.DTO.Room;

namespace HMSRoomManagement.Service.Contracts
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetRoomsAsync(Guid? hotelId, int? roomTypeId);
        Task<RoomDto> GetRoomByIdAsync(int id);
        Task<RoomDto> CreateRoomAsync(CreateRoomDto dto);
        Task UpdateRoomAsync(int id, UpdateRoomDto dto);
        Task UpdateRoomStatusAsync(int id, string status);
        Task DeleteRoomAsync(int id);
    }
}

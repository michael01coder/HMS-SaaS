using HMSRoomManagement.DTO.RoomType;

namespace HMSRoomManagement.Service.Contracts
{
    public interface IRoomTypeService
    {
        Task<IEnumerable<RoomTypeDto>> GetRoomTypesAsync(Guid? hotelId);
        Task<RoomTypeDto> GetRoomTypeByIdAsync(int id);
        Task<RoomTypeDto> CreateRoomTypeAsync(CreateRoomTypeDto dto);
        Task UpdateRoomTypeAsync(int id, UpdateRoomTypeDto dto);
        Task DeleteRoomTypeAsync(int id);
    }
}

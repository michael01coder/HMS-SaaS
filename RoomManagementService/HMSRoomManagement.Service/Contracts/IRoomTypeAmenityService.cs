using HMSRoomManagement.DTO.RoomTypeAmenity;

namespace HMSRoomManagement.Service.Contracts
{
    public interface IRoomTypeAmenityService
    {
        Task<IEnumerable<RoomTypeAmenityDto>> GetLinksAsync(int? roomTypeId);
        Task<RoomTypeAmenityDto> LinkAsync(RoomTypeAmenityDto dto);
        Task UnlinkAsync(int roomTypeId, int amenityId);
    }
}

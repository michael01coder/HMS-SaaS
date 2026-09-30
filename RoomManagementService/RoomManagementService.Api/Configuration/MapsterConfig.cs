using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.DTO.RoomDTO;
using HMSRoomManagement.DTO.RoomTypeDTO;
using Mapster;

namespace RoomManagementService.Api.Configuration
{
    public class MapsterConfig
    {
        public static void RegisterMapping(TypeAdapterConfig config)
        {
            
            TypeAdapterConfig<CreateRoomTypeDTO, RoomType>.NewConfig();
            TypeAdapterConfig<UpdateRoomTypeDTO, RoomType>.NewConfig();
            TypeAdapterConfig<RoomType, RoomTypeDTO>.NewConfig();

            TypeAdapterConfig<CreateRoomDTO, Room>.NewConfig();
            TypeAdapterConfig<UpdateRoomDTO, Room>.NewConfig();
            TypeAdapterConfig<Room, RoomDTO>.NewConfig();
        }
    }
}

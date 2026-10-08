using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.DTO.Room;
using HMSRoomManagement.DTO.RoomType;
using Mapster;

namespace RoomManagementService.Api.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            config.NewConfig<RoomType, RoomTypeDto>();
            config.NewConfig<CreateRoomTypeDto, RoomType>();
            config.NewConfig<UpdateRoomTypeDto, RoomType>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.HotelId);

            config.NewConfig<Room, RoomDto>();
            config.NewConfig<CreateRoomDto, Room>();
            config.NewConfig<UpdateRoomDto, Room>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.HotelId);

            // RatePlan/CancellationPolicy are composed by hand in
            // RatePlanService (they're two tables flattened into one DTO),
            // so no Mapster config is registered for them.
        }
    }
}


using Mapster;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO.HouseKeepingDTO;
using HMS.OperationalService.DTO.MaintainanceReqDTO;
using HMS.OperationalService.DTO.ServiceReqDTO;
using HMS.OperationalService.DTO.LostAndFoundDTO;
using HMS.OperationalService.DTO.RoomInspectionDTO;
using HMS.OperationalService.DTO.StaffTaskAssignmentDTO;

namespace HMS.OperationalService.Api.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            config.NewConfig<HouseKeepingTask, HouseKeepingTaskResponseDTO>();
            config.NewConfig<CreateHouseKeepingTaskDTO, HouseKeepingTask>();


            config.NewConfig<MaintainanceRequest, MaintainanceReqDTO>();
            config.NewConfig<CreateMaintainanceReqDTO, MaintainanceRequest>();

            config.NewConfig<ServiceRequest, ServiceReqDTO>();
            config.NewConfig<CreateServiceReqDTO, ServiceRequest>();

            config.NewConfig<LostAndFound, LostAndFoundDTO>();
            config.NewConfig<CreateLostandFoundDTO, LostAndFound>();

            config.NewConfig<RoomInspection, RoomInspectionDTO>();
            config.NewConfig<CreateRoomInspectionDTO, RoomInspection>();

            config.NewConfig<StaffTaskAssignment, StaffTaskAssignmentDTO>();
            config.NewConfig<CreateStaffTaskAssignmentDTO, StaffTaskAssignment>();

        }
    }
}

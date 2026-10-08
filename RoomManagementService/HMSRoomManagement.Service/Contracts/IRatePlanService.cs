using HMSRoomManagement.DTO.RatePlan;

namespace HMSRoomManagement.Service.Contracts
{
    public interface IRatePlanService
    {
        Task<IEnumerable<RatePlanDto>> GetRatePlansAsync(int? roomTypeId);
        Task<RatePlanDto> GetRatePlanByIdAsync(int id);
        Task<RatePlanDto> CreateRatePlanAsync(CreateRatePlanDto dto);
        Task UpdateRatePlanAsync(int id, UpdateRatePlanDto dto);
        Task DeleteRatePlanAsync(int id);
    }
}

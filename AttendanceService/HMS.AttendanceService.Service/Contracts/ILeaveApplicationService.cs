using HMS.AttendanceService.DTO.Leave;

namespace HMS.AttendanceService.Service.Contracts
{
    public interface ILeaveApplicationService
    {
        Task<LeaveApplicationDto> ApplyLeaveAsync(CreateLeaveApplicationDto dto);
        Task<LeaveApplicationDto> GetByIdAsync(Guid id);
        Task<IEnumerable<LeaveApplicationDto>> GetByHotelAsync();
        Task<IEnumerable<LeaveApplicationDto>> GetByStaffAsync(Guid staffId);
        Task<IEnumerable<LeaveApplicationDto>> GetPendingForHotelAsync();
        Task<LeaveApplicationDto> UpdateStatusAsync(Guid id, UpdateLeaveStatusDto dto);
        Task DeleteAsync(Guid id);

        Task<LeaveBalanceDto> GetStaffLeaveBalanceAsync(Guid staffId, string leaveTypeName, int year);
    }
}
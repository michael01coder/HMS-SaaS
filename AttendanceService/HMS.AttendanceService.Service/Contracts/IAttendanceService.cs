using HMS.AttendanceService.DTO.Attendance;

namespace HMS.AttendanceService.Service.Contracts
{
    public interface IAttendanceService
    {
        Task<AttendanceDto> ClockInAsync(ClockInDto dto);
        Task<AttendanceDto> ClockOutAsync(ClockOutDto dto);
        Task<IEnumerable<AttendanceDto>> GetDailyAttendanceAsync(DateOnly date);
        Task<IEnumerable<AttendanceDto>> GetStaffHistoryAsync(Guid staffId, DateOnly startDate, DateOnly endDate);
        Task<AttendanceDto> ManualCreateAsync(CreateAttendanceDto dto);
        Task UpdateAsync(Guid id, UpdateAttendanceDto dto);
    }
}
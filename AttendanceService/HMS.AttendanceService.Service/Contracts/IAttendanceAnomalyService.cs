using HMS.AttendanceService.DTO.Anomaly;

namespace HMS.AttendanceService.Service.Contracts
{
    public interface IAttendanceAnomalyService
    {
        Task<AttendanceAnomalyDto> CreateAnomalyAsync(CreateAttendanceAnomalyDto dto);
        Task<IEnumerable<AttendanceAnomalyDto>> GetAnomaliesByAttendanceIdAsync(Guid attendanceId);
        Task<IEnumerable<AttendanceAnomalyDto>> GetAnomaliesByStaffIdAsync(Guid staffId);
        Task<IEnumerable<AttendanceAnomalyDto>> GetPendingAnomaliesForHotelAsync();
        Task UpdateAnomalyAsync(Guid id, UpdateAttendanceAnomalyDto dto);
        Task DeleteAnomalyAsync(Guid id);
    }
}
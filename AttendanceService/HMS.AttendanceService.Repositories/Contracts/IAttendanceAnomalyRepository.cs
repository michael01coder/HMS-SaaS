using HMS.AttendanceService.Domain.Data;
using HMS.AttendanceService.Repository.Contracts;

namespace HMS.AttendanceService.Repository.Contracts
{
    public interface IAttendanceAnomalyRepository : IGenericRepository<AttendanceAnomaly, Guid>
    {
        Task<List<AttendanceAnomaly>> GetByAttendanceIdAsync(Guid attendanceId);
        Task<List<AttendanceAnomaly>> GetByStaffIdAsync(Guid staffId);
        Task<List<AttendanceAnomaly>> GetPendingAnomaliesByHotelAsync(Guid hotelId);
    }
}
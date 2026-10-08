using HMS.AttendanceService.Domain.Data;

namespace HMS.AttendanceService.Repository.Contracts
{
    public interface IAttendanceRepository : IGenericRepository<Attendance, Guid>
    {
        Task<Attendance?> GetTodayRecordAsync(Guid hotelId, Guid staffId, DateOnly date);
        Task<List<Attendance>> GetHotelDailyAttendanceAsync(Guid hotelId, DateOnly date);
        Task<List<Attendance>> GetStaffHistoryAsync(Guid hotelId, Guid staffId, DateOnly startDate, DateOnly endDate);
    }
}
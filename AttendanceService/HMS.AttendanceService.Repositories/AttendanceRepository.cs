using HMS.AttendanceService.Domain;
using HMS.AttendanceService.Domain.Data;
using HMS.AttendanceService.Repository.Contracts;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.AttendanceService.Repository
{
    public class AttendanceRepository : GenericRepository<Attendance, Guid>, IAttendanceRepository
    {
        public AttendanceRepository(AttendanceDbContext context, IMapper mapper)
            : base(context, mapper) { }

        public async Task<Attendance?> GetTodayRecordAsync(Guid hotelId, Guid staffId, DateOnly date)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.HotelId == hotelId && a.StaffId == staffId && a.AttendanceDate == date);
        }

        public async Task<List<Attendance>> GetHotelDailyAttendanceAsync(Guid hotelId, DateOnly date)
        {
            return await _dbSet.AsNoTracking()
                .Include(a => a.Anomalies)
                .Where(a => a.HotelId == hotelId && a.AttendanceDate == date)
                .ToListAsync();
        }

        public async Task<List<Attendance>> GetStaffHistoryAsync(Guid hotelId, Guid staffId, DateOnly startDate, DateOnly endDate)
        {
            return await _dbSet.AsNoTracking()
                .Where(a => a.HotelId == hotelId && a.StaffId == staffId && a.AttendanceDate >= startDate && a.AttendanceDate <= endDate)
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync();
        }
    }
}
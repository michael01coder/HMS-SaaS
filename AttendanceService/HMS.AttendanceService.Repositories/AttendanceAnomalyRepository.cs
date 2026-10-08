using HMS.AttendanceService.Domain;
using HMS.AttendanceService.Domain.Data;
using HMS.AttendanceService.Repository.Contracts;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.AttendanceService.Repository
{
    public class AttendanceAnomalyRepository : GenericRepository<AttendanceAnomaly, Guid>, IAttendanceAnomalyRepository
    {
        public AttendanceAnomalyRepository(AttendanceDbContext context, IMapper mapper)
            : base(context, mapper) { }

        public async Task<List<AttendanceAnomaly>> GetByAttendanceIdAsync(Guid attendanceId)
        {
            return await _dbSet.AsNoTracking()
                .Where(a => a.AttendanceId == attendanceId)
                .ToListAsync();
        }

        public async Task<List<AttendanceAnomaly>> GetByStaffIdAsync(Guid staffId)
        {
            return await _dbSet.AsNoTracking()
                .Where(a => a.StaffId == staffId)
                .OrderByDescending(a => a.ReportedAt)
                .ToListAsync();
        }

        public async Task<List<AttendanceAnomaly>> GetPendingAnomaliesByHotelAsync(Guid hotelId)
        {
            return await _dbSet.AsNoTracking()
                .Include(a => a.Attendance)
                .Where(a => a.Attendance != null && a.Attendance.HotelId == hotelId && a.Status == "Pending")
                .OrderByDescending(a => a.ReportedAt)
                .ToListAsync();
        }
    }
}
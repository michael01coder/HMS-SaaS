using HMS.AttendanceService.Domain;
using HMS.AttendanceService.Domain.Data;
using HMS.AttendanceService.Repository.Contracts;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.AttendanceService.Repository
{
    public class LeaveApplicationRepository : GenericRepository<LeaveApplication, Guid>, ILeaveApplicationRepository
    {
        public LeaveApplicationRepository(AttendanceDbContext context, IMapper mapper)
            : base(context, mapper) { }

        public async Task<bool> HasOverlapAsync(Guid hotelId, Guid staffId, DateOnly start, DateOnly end)
        {
            return await _dbSet.AnyAsync(l => l.HotelId == hotelId && l.StaffId == staffId
                && (l.Status == "Pending" || l.Status == "Approved")
                && l.StartDate <= end && l.EndDate >= start);
        }

        public async Task<List<LeaveApplication>> GetByHotelIdAsync(Guid hotelId)
        {
            return await _dbSet.AsNoTracking()
                .Where(l => l.HotelId == hotelId)
                .OrderByDescending(l => l.AppliedOn)
                .ToListAsync();
        }

        public async Task<List<LeaveApplication>> GetByStaffIdAsync(Guid hotelId, Guid staffId)
        {
            return await _dbSet.AsNoTracking()
                .Where(l => l.HotelId == hotelId && l.StaffId == staffId)
                .OrderByDescending(l => l.AppliedOn)
                .ToListAsync();
        }

        public async Task<List<LeaveApplication>> GetPendingByHotelIdAsync(Guid hotelId)
        {
            return await _dbSet.AsNoTracking()
                .Where(l => l.HotelId == hotelId && l.Status == "Pending")
                .OrderByDescending(l => l.AppliedOn)
                .ToListAsync();
        }

        public async Task<List<LeaveApplication>> GetApprovedLeavesForYearAsync(Guid hotelId, Guid staffId, string leaveTypeName, int year)
        {
            return await _dbSet.AsNoTracking()
                .Where(l => l.HotelId == hotelId &&
                            l.StaffId == staffId &&
                            l.LeaveTypeName == leaveTypeName &&
                            l.Status == "Approved" &&
                            l.StartDate.Year == year)
                .ToListAsync();
        }
    }
}
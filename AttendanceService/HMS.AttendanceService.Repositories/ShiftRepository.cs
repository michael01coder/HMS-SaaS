using HMS.AttendanceService.Domain;
using HMS.AttendanceService.Domain.Data;
using HMS.AttendanceService.Repository.Contracts;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.AttendanceService.Repository
{
    public class ShiftRepository : GenericRepository<Shift, Guid>, IShiftRepository
    {
        public ShiftRepository(AttendanceDbContext context, IMapper mapper) : base(context, mapper) { }

        public async Task<List<Shift>> GetActiveShiftsByHotelAsync(Guid hotelId)
        {
            return await _dbSet.AsNoTracking()
                .Where(s => s.HotelId == hotelId && s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }
    }

    public class ShiftAllocationRepository : GenericRepository<ShiftAllocation, Guid>, IShiftAllocationRepository
    {
        public ShiftAllocationRepository(AttendanceDbContext context, IMapper mapper) : base(context, mapper) { }

        public async Task<List<ShiftAllocation>> GetByHotelAndDateRangeAsync(Guid hotelId, DateOnly startDate, DateOnly endDate)
        {
            return await _dbSet.AsNoTracking()
                .Include(a => a.Shift)
                .Where(a => a.HotelId == hotelId && a.WorkDate >= startDate && a.WorkDate <= endDate && !a.IsDeleted)
                .OrderBy(a => a.WorkDate)
                .ToListAsync();
        }

        public async Task<List<ShiftAllocation>> GetByStaffAndDateRangeAsync(Guid hotelId, Guid staffId, DateOnly startDate, DateOnly endDate)
        {
            return await _dbSet.AsNoTracking()
                .Include(a => a.Shift)
                .Where(a => a.HotelId == hotelId && a.StaffId == staffId && a.WorkDate >= startDate && a.WorkDate <= endDate && !a.IsDeleted)
                .OrderBy(a => a.WorkDate)
                .ToListAsync();
        }

        public async Task<bool> HasConflictAsync(Guid hotelId, Guid staffId, DateOnly workDate)
        {
            return await _dbSet.AnyAsync(a => a.HotelId == hotelId && a.StaffId == staffId && a.WorkDate == workDate && !a.IsDeleted);
        }
    }

    public class ShiftSwapRequestRepository : GenericRepository<ShiftSwapRequest, Guid>, IShiftSwapRequestRepository
    {
        public ShiftSwapRequestRepository(AttendanceDbContext context, IMapper mapper) : base(context, mapper) { }

        public async Task<List<ShiftSwapRequest>> GetPendingByHotelAsync(Guid hotelId)
        {
            return await _dbSet.AsNoTracking()
                .Include(s => s.RequesterAllocation).ThenInclude(ra => ra!.Shift)
                .Include(s => s.TargetAllocation).ThenInclude(ta => ta!.Shift)
                .Where(s => s.HotelId == hotelId && s.Status == "Pending" && !s.IsDeleted)
                .OrderByDescending(s => s.RequestedAt)
                .ToListAsync();
        }

        public async Task<List<ShiftSwapRequest>> GetByStaffAsync(Guid hotelId, Guid staffId)
        {
            return await _dbSet.AsNoTracking()
                .Include(s => s.RequesterAllocation).ThenInclude(ra => ra!.Shift)
                .Include(s => s.TargetAllocation).ThenInclude(ta => ta!.Shift)
                .Where(s => s.HotelId == hotelId && (s.RequesterStaffId == staffId || s.TargetStaffId == staffId) && !s.IsDeleted)
                .OrderByDescending(s => s.RequestedAt)
                .ToListAsync();
        }
    }
}
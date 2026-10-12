using HMS.AttendanceService.Domain.Data;

namespace HMS.AttendanceService.Repository.Contracts
{
    public interface IShiftRepository : IGenericRepository<Shift, Guid>
    {
        Task<List<Shift>> GetActiveShiftsByHotelAsync(Guid hotelId);
    }

    public interface IShiftAllocationRepository : IGenericRepository<ShiftAllocation, Guid>
    {
        Task<List<ShiftAllocation>> GetByHotelAndDateRangeAsync(Guid hotelId, DateOnly startDate, DateOnly endDate);
        Task<List<ShiftAllocation>> GetByStaffAndDateRangeAsync(Guid hotelId, Guid staffId, DateOnly startDate, DateOnly endDate);
        Task<bool> HasConflictAsync(Guid hotelId, Guid staffId, DateOnly workDate);
    }

    public interface IShiftSwapRequestRepository : IGenericRepository<ShiftSwapRequest, Guid>
    {
        Task<List<ShiftSwapRequest>> GetPendingByHotelAsync(Guid hotelId);
        Task<List<ShiftSwapRequest>> GetByStaffAsync(Guid hotelId, Guid staffId);
    }
}
using HMS.AttendanceService.Domain.Data;

namespace HMS.AttendanceService.Repository.Contracts
{
    public interface ILeaveApplicationRepository : IGenericRepository<LeaveApplication, Guid>
    {
        Task<List<LeaveApplication>> GetByHotelIdAsync(Guid hotelId);
        Task<List<LeaveApplication>> GetByStaffIdAsync(Guid hotelId, Guid staffId);
        Task<List<LeaveApplication>> GetPendingByHotelIdAsync(Guid hotelId);

        // Add this method
        Task<List<LeaveApplication>> GetApprovedLeavesForYearAsync(Guid hotelId, Guid staffId, string leaveTypeName, int year);
    }
}
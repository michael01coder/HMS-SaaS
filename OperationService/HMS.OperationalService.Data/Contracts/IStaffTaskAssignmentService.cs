using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;

namespace HMS.OperationalService.Service.Contracts
{
    public interface IStaffTaskAssignmentService
    {
        Task<IEnumerable<StaffTaskAssignment>> GetAllAsync();
        Task<StaffTaskAssignment> GetByIdAsync(int id);
        Task<StaffTaskAssignment> CreateAsync(StaffTaskAssignmentDto dto);
        Task UpdateAsync(int id, StaffTaskAssignmentDto dto);
        Task DeleteAsync(int id);
    }
}

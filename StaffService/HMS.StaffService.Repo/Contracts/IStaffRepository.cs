using HMS.StaffService.Domain.Data;

namespace HMS.StaffService.Repository.Contracts
{
    public interface IStaffRepository : IGenericRepository<Staff, Guid>
    {
        Task<Staff?> GetWithDetailsAsync(Guid staffId);
        Task<List<Staff>> GetByDepartmentIdAsync(int departmentId);
    }
}
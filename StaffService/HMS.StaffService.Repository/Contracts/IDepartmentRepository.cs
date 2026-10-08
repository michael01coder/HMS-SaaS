using HMS.StaffService.Domain.Data;

namespace HMS.StaffService.Repository.Contracts
{
    public interface IDepartmentRepository : IGenericRepository<Department, int>
    {
        Task<List<Department>> GetByHotelIdAsync(Guid hotelId);
    }
}
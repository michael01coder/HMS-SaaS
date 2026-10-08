using HMS.StaffService.DTO.Department;

namespace HMS.StaffService.Service.Contracts
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
        Task<DepartmentDto> GetDepartmentByIdAsync(int id);
        Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto);
        Task UpdateDepartmentAsync(int id, BaseDepartmentDto dto);
        Task DeleteDepartmentAsync(int id);
    }
}
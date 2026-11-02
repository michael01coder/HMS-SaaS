using HMS.StaffService.DTO.Department;

namespace HMS.StaffService.Service.Contracts
{
    public interface IDesignationService
    {
        Task<IEnumerable<DesignationDto>> GetAllAsync();
        Task<DesignationDto> GetByIdAsync(int id);
        Task<DesignationDto> CreateAsync(CreateDesignationDto dto);
        Task UpdateAsync(int id, BaseDesignationDto dto);
        Task DeleteAsync(int id);
    }
}

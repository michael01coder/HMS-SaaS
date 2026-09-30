using HMS.StaffService.DTO.Staff;
namespace HMS.StaffService.Service.Contracts
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffDto>> GetAllStaffAsync();
        Task<StaffDto> GetStaffByIdAsync(Guid id);
        Task<StaffDto> CreateStaffAsync(CreateStaffDto dto);
        Task UpdateStaffAsync(Guid id, UpdateStaffDto dto);
        Task DeleteStaffAsync(Guid id);
    }
}
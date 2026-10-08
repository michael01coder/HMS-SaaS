using HMS.StaffService.DTO.Staff;
namespace HMS.StaffService.Service.Contracts
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffDto>> GetAllStaffAsync();
        Task<StaffDto> GetStaffByIdAsync(Guid id);
        Task<StaffDto> CreateStaffAsync(CreateStaffDto dto);

        /// <summary>
        /// Called only by AuthService's internal client, never by a human —
        /// see InternalStaffController. Builds a minimal Staff row pinned to
        /// an explicit StaffId, with placeholder values for the HR-only
        /// fields Auth never collects (EmployeeCode, Designation, etc.).
        /// </summary>
        Task<StaffDto> CreateStaffFromAuthAsync(InternalCreateStaffDto dto);
        Task UpdateStaffAsync(Guid id, UpdateStaffDto dto);
        Task DeleteStaffAsync(Guid id);
    }
}
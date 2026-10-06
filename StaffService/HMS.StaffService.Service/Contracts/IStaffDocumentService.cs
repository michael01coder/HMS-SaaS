using HMS.StaffService.DTO.StaffDocument;

namespace HMS.StaffService.Service.Contracts
{
    public interface IStaffDocumentService
    {
        Task<IEnumerable<StaffDocumentDto>> GetAllAsync();
        Task<IEnumerable<StaffDocumentDto>> GetByStaffIdAsync(Guid staffId);
        Task<StaffDocumentDto> GetByIdAsync(Guid id);
        Task<StaffDocumentDto> CreateAsync(CreateStaffDocumentDto dto);
        Task UpdateAsync(Guid id, UpdateStaffDocumentDto dto);
        Task DeleteAsync(Guid id);
    }
}
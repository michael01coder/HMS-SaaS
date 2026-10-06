using HMS.StaffService.DTO.StaffNote;

namespace HMS.StaffService.Service.Contracts
{
    public interface IStaffNotesService
    {
        Task<IEnumerable<StaffNoteDto>> GetAllAsync();
        Task<IEnumerable<StaffNoteDto>> GetByStaffIdAsync(Guid staffId);
        Task<StaffNoteDto> GetByIdAsync(Guid id);
        Task<StaffNoteDto> CreateAsync(CreateStaffNoteDto dto);
        Task UpdateAsync(Guid id, UpdateStaffNoteDto dto);
        Task DeleteAsync(Guid id);
    }
}
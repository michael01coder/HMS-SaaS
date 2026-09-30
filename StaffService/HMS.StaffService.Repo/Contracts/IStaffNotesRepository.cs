using HMS.StaffService.Domain.Data;

namespace HMS.StaffService.Repository.Contracts
{
    public interface IStaffNotesRepository : IGenericRepository<StaffNotes, Guid>
    {
        Task<List<StaffNotes>> GetByStaffIdAsync(Guid staffId);
    }
}
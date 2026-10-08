using HMS.StaffService.Domain.Data;

namespace HMS.StaffService.Repository.Contracts
{
    public interface IStaffDocumentRepository : IGenericRepository<StaffDocument, Guid>
    {
        Task<List<StaffDocument>> GetByStaffIdAsync(Guid staffId);
    }
}
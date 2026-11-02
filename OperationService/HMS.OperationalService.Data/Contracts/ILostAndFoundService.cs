using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;

namespace HMS.OperationalService.Service.Contracts
{
    public interface ILostAndFoundService
    {
        Task<IEnumerable<LostAndFound>> GetAllAsync();
        Task<LostAndFound> GetByIdAsync(int id);
        Task<LostAndFound> CreateAsync(LostAndFoundDto dto);
        Task ClaimItemAsync(int id, Guid guestId);
        Task DeleteAsync(int id);
    }
}

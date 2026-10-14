using HMS.OperationalService.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Service.Contracts
{
    public interface ILostAndFoundService
    {
        Task<IEnumerable<LostAndFound>> GetAllAsync();
        Task<LostAndFound> GetByIdAsync(int id);
        Task<LostAndFound> CreateAsync(LostAndFound item);
        Task ClaimItemAsync(int id, Guid guestId);
        Task DeleteAsync(int id);
    }
}

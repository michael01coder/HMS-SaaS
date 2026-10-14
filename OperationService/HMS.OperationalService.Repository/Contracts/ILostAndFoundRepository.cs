using HMS.OperationalService.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository.Contracts
{
    public interface ILostAndFoundRepository : IGenericRepository<LostAndFound , int>
    {
        Task<IEnumerable<LostAndFound>> GetUnclaimedItemsAsync();
    }
}

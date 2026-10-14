using HMS.OperationalService.Domain;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository
{
    public class LostAndFoundRepository : GenericRepository<LostAndFound , int>, ILostAndFoundRepository
    {
        public LostAndFoundRepository(OperationalDbContext context) : base(context) { }

        public async Task<IEnumerable<LostAndFound>> GetUnclaimedItemsAsync()
        {
            return await _dbSet.Where(x => !x.IsClaimed).ToListAsync();
        }
    }
}

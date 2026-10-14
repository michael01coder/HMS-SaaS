using HMS.OperationalService.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository.Contracts
{
    public interface IHouseKeepingRepository : IGenericRepository<HouseKeepingTask, int> 
    {
        Task<IEnumerable<HouseKeepingTask>> GetTasksByStaffIdAsync(Guid staffId);
    }

}
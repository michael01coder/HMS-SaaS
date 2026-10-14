using HMS.OperationalService.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository.Contracts
{
    public interface IStaffTaskAssignmentRepository : IGenericRepository<StaffTaskAssignment , Guid>
    {
        Task<IEnumerable<StaffTaskAssignment>> GetAssignmentsByStaffAsync(Guid staffId);
    }
}

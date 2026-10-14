using HMS.OperationalService.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Service.Contracts
{
    public interface IStaffTaskAssignmentService
    {
        Task<IEnumerable<StaffTaskAssignment>> GetAllAsync();
        Task<StaffTaskAssignment> GetByIdAsync(int id);
        Task<StaffTaskAssignment> AssignTaskAsync(StaffTaskAssignment assignment);
        Task UpdateAssignmentAsync(StaffTaskAssignment assignment);
        Task DeleteAsync(int id);
    }
}

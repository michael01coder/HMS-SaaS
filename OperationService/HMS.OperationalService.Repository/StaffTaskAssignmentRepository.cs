using HMS.OperationalService.Domain;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;

namespace HMS.OperationalService.Repository
{
    public class StaffTaskAssignmentRepository : GenericRepository<StaffTaskAssignment, int>, IStaffTaskAssignmentRepository
    {
        public StaffTaskAssignmentRepository(OperationalDbContext context) : base(context) { }
    }
}

using HMS.OperationalService.Domain;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;

namespace HMS.OperationalService.Repository
{
    public class MaintainanceRepository : GenericRepository<MaintainanceRequest, int>, IMaintainanceRepository
    {
        public MaintainanceRepository(OperationalDbContext context) : base(context) { }
    }
}

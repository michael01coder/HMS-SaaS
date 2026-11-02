using HMS.OperationalService.Domain;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;

namespace HMS.OperationalService.Repository
{
    public class HouseKeepingRepository : GenericRepository<HouseKeepingTask, int>, IHouseKeepingRepository
    {
        public HouseKeepingRepository(OperationalDbContext context) : base(context) { }
    }
}

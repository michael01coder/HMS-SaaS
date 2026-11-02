using HMS.OperationalService.Domain;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;

namespace HMS.OperationalService.Repository
{
    public class RoomInspectionRepository : GenericRepository<RoomInspection, int>, IRoomInspectionRepository
    {
        public RoomInspectionRepository(OperationalDbContext context) : base(context) { }
    }
}

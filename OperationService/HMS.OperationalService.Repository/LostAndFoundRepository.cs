using HMS.OperationalService.Domain;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;

namespace HMS.OperationalService.Repository
{
    public class LostAndFoundRepository : GenericRepository<LostAndFound, int>, ILostAndFoundRepository
    {
        public LostAndFoundRepository(OperationalDbContext context) : base(context) { }
    }
}

using HMS.OperationalService.Domain;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;

namespace HMS.OperationalService.Repository
{
    public class ServiceRequestRepository : GenericRepository<ServiceRequest, int>, IServiceRequestRepository
    {
        public ServiceRequestRepository(OperationalDbContext context) : base(context) { }
    }
}

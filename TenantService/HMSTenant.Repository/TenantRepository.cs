using HMSTenant.Domain;
using HMSTenant.Domain.Data;
using HMSTenant.Repository.Contracts;
using MapsterMapper;

namespace HMSTenant.Repository
{
    public class TenantRepository : GenericRepository<Tenant, Guid>, ITenantRepository
    {
        public TenantRepository(TenantSerrviceDbContext context, IMapper mapper) : base(context, mapper) { }
    }
}

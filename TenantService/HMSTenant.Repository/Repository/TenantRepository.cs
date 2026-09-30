

using HMSTenant.Data;
using HMSTenant.Domain.Data;
using HMSTenant.Repository.Contract;


namespace HMSTenant.Repository.Repository
{
    public class TenantRepository : GenericRepository<Tenant, Guid>, ITenantRepository
    {
        public TenantRepository(TenantSerrviceDbContext context) : base(context) { }
    }
}

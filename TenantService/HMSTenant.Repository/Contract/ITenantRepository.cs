
using HMSTenant.Domain.Data;

namespace HMSTenant.Repository.Contract
{
    public interface ITenantRepository : IGenericRepository<Tenant, Guid>
    {
    }
}

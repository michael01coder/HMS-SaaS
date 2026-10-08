using HMSTenant.Domain.Data;

namespace HMSTenant.Repository.Contracts
{
    public interface ITenantRepository : IGenericRepository<Tenant, Guid>
    {
    }
}

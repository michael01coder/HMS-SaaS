using HMSTenant.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace HMSTenant.Domain
{
    public class TenantSerrviceDbContext : DbContext
    {
        public TenantSerrviceDbContext(DbContextOptions<TenantSerrviceDbContext> options) : base(options) { }

        public DbSet<Tenant> Tenants => Set<Tenant>();
    }
}

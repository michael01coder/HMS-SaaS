using HMS.StaffService.Domain;
using HMS.StaffService.Domain.Data;
using HMS.StaffService.Repository.Contracts;
using MapsterMapper;

namespace HMS.StaffService.Repository
{
    public class DesignationRepository : GenericRepository<Designation, int>, IDesignationRepository
    {
        public DesignationRepository(StaffDbContext context, IMapper mapper) : base(context, mapper) { }
    }
}

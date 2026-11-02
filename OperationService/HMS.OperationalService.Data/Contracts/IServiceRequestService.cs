using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;

namespace HMS.OperationalService.Service.Contracts
{
    public interface IServiceRequestService
    {
        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest> GetByIdAsync(int id);
        Task<ServiceRequest> CreateAsync(ServiceRequestDto dto);
        Task UpdateAsync(int id, ServiceRequestDto dto);
        Task DeleteAsync(int id);
    }
}

using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Service
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _repository;

        public ServiceRequestService(IServiceRequestRepository repository) => _repository = repository;

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<ServiceRequest> GetByIdAsync(int id) =>
            await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"Service request with ID {id} not found.");

        public async Task<ServiceRequest> CreateAsync(ServiceRequest request)
        {
            request.RequestCode = "SRV-" + Guid.NewGuid().ToString()[..6].ToUpper();
            request.RequestedAt = DateTime.UtcNow;
            request.CreatedAt = DateTime.UtcNow;
            await _repository.AddAsync(request);
            await _repository.SaveChangesAsync();
            return request;
        }

        public async Task UpdateAsync(ServiceRequest request)
        {
            request.UpdatedAt = DateTime.UtcNow;
            if (request.Status == Common.Enums.TaskStatus.Completed && request.CompletedAt == null)
            {
                request.CompletedAt = DateTime.UtcNow;
            }
            _repository.Update(request);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await GetByIdAsync(id);
            _repository.Delete(item);
            await _repository.SaveChangesAsync();
        }
    }
}

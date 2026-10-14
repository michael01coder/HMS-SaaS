using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using Microsoft.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Service
{
    public class MaintainanceService : IMaintainanceService
    {
        private readonly IMaintainanceRepository _repository;

        public MaintainanceService(IMaintainanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MaintainanceRequest>> GetAllRequestsAsync() => await _repository.GetAllAsync();

        public async Task<MaintainanceRequest> GetRequestByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"Maintenance request with ID {id} not found.");
        }

        public async Task<MaintainanceRequest> CreateRequestAsync(MaintainanceRequest request)
        {
            request.RequestCode = "MNT-" + Guid.NewGuid().ToString()[..6].ToUpper();
            request.CreatedAt = DateTime.UtcNow;
            await _repository.AddAsync(request);
            await _repository.SaveChangesAsync();
            return request;
        }

        public async Task UpdateRequestAsync(MaintainanceRequest request)
        {
            request.UpdatedAt = DateTime.UtcNow;
            if (request.Status == HMS.OperationalService.Common.Enums.TaskStatus.Resolved && request.ResolvedAt == null)
            {
                request.ResolvedAt = DateTime.UtcNow;
            }
            _repository.Update(request);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteRequestAsync(int id)
        {
            var req = await GetRequestByIdAsync(id);
            _repository.Delete(req);
            await _repository.SaveChangesAsync();
        }
    }
}

using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Service
{
    public class RoomInspectionService : IRoomInspectionService
    {
        private readonly IRoomInspectionRepository _repository;

        public RoomInspectionService(IRoomInspectionRepository repository) => _repository = repository;

        public async Task<IEnumerable<RoomInspection>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<RoomInspection> GetByIdAsync(int id) =>
            await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"Inspection with ID {id} not found.");

        public async Task<RoomInspection> CreateAsync(RoomInspection inspection)
        {
            inspection.InspectionCode = "INS-" + Guid.NewGuid().ToString()[..6].ToUpper();
            inspection.InspectionDate = DateTime.UtcNow;
            inspection.CreatedAt = DateTime.UtcNow;
            await _repository.AddAsync(inspection);
            await _repository.SaveChangesAsync();
            return inspection;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await GetByIdAsync(id);
            _repository.Delete(item);
            await _repository.SaveChangesAsync();
        }
    }
}

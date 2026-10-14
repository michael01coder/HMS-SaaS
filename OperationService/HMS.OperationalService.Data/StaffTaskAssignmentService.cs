using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Service
{
     public class StaffTaskAssignmentService : IStaffTaskAssignmentService
        {
            private readonly IStaffTaskAssignmentRepository _repository;

            public StaffTaskAssignmentService(IStaffTaskAssignmentRepository repository) => _repository = repository;

            public async Task<IEnumerable<StaffTaskAssignment>> GetAllAsync() => await _repository.GetAllAsync();

            public async Task<StaffTaskAssignment> GetByIdAsync(int id) =>
                await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"Assignment with ID {id} not found.");

            public async Task<StaffTaskAssignment> AssignTaskAsync(StaffTaskAssignment assignment)
            {
                assignment.AssignedAt = DateTime.UtcNow;
                await _repository.AddAsync(assignment);
                await _repository.SaveChangesAsync();
                return assignment;
            }

            public async Task UpdateAssignmentAsync(StaffTaskAssignment assignment)
            {
                _repository.Update(assignment);
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


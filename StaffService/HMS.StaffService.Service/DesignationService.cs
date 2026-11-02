using HMS.StaffService.Common.Exceptions;
using HMS.StaffService.Common.Service;
using HMS.StaffService.DTO.Department;
using HMS.StaffService.Repository.Contracts;
using HMS.StaffService.Service.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HMS.StaffService.Service
{
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationRepository _repository;
        private readonly IDepartmentRepository _departments;
        private readonly ICurrentUserService _currentUser;

        public DesignationService(IDesignationRepository repository, IDepartmentRepository departments, ICurrentUserService currentUser)
        {
            _repository = repository;
            _departments = departments;
            _currentUser = currentUser;
        }

        private Guid HotelId => _currentUser.HotelId ?? throw new ForbiddenException("Missing HotelId claim.");

        public async Task<IEnumerable<DesignationDto>> GetAllAsync()
        {
            var hotelId = HotelId;
            return await _repository.Query()
                .Where(d => d.HotelId == hotelId)
                .OrderBy(d => d.DesignationName)
                .Select(d => new DesignationDto
                {
                    DesignationId = d.DesignationId,
                    HotelId = d.HotelId,
                    DesignationName = d.DesignationName,
                    DepartmentId = d.DepartmentId,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<DesignationDto> GetByIdAsync(int id)
        {
            var item = await GetOwnedAsync(id);
            return new DesignationDto
            {
                DesignationId = item.DesignationId,
                HotelId = item.HotelId,
                DesignationName = item.DesignationName,
                DepartmentId = item.DepartmentId,
                IsActive = item.IsActive,
                CreatedAt = item.CreatedAt
            };
        }

        public async Task<DesignationDto> CreateAsync(CreateDesignationDto dto)
        {
            dto.HotelId = HotelId;
            await EnsureDepartmentAsync(dto.DepartmentId);
            await EnsureUniqueAsync(dto.DesignationName, dto.DepartmentId, null);
            return await _repository.AddAsync<CreateDesignationDto, DesignationDto>(dto);
        }

        public async Task UpdateAsync(int id, BaseDesignationDto dto)
        {
            await GetOwnedAsync(id);
            await EnsureDepartmentAsync(dto.DepartmentId);
            await EnsureUniqueAsync(dto.DesignationName, dto.DepartmentId, id);
            await _repository.UpdateAsync(id, dto);
        }

        public async Task DeleteAsync(int id)
        {
            await GetOwnedAsync(id);
            await _repository.DeleteAsync(id);
        }

        private async Task<Domain.Data.Designation> GetOwnedAsync(int id)
        {
            var item = await _repository.GetAsync(id)
                ?? throw new NotFoundException($"Designation '{id}' not found.");
            if (item.HotelId != HotelId) throw new ForbiddenException("Access denied.");
            return item;
        }

        private async Task EnsureDepartmentAsync(int? departmentId)
        {
            if (!departmentId.HasValue) return;
            var department = await _departments.GetAsync(departmentId.Value)
                ?? throw new BadRequestException("Department does not exist.");
            if (department.HotelId != HotelId) throw new BadRequestException("Department belongs to a different hotel.");
        }

        private async Task EnsureUniqueAsync(string name, int? departmentId, int? ignoreId)
        {
            var hotelId = HotelId;
            var clash = await _repository.ExistsAsync(d =>
                d.HotelId == hotelId
                && d.DepartmentId == departmentId
                && d.DesignationName == name
                && (!ignoreId.HasValue || d.DesignationId != ignoreId.Value));
            if (clash) throw new BadRequestException("This designation already exists.");
        }
    }
}

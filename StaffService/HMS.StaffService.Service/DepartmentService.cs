using HMS.StaffService.Common.Exceptions;
using HMS.StaffService.Common.Service;
using HMS.StaffService.Service.Contracts;
using HMS.StaffService.DTO;
using HMS.StaffService.DTO.Department;
using HMS.StaffService.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HMS.StaffService.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ICurrentUserService _currentUserService;

        public DepartmentService(IDepartmentRepository departmentRepository, ICurrentUserService currentUserService)
        {
            _departmentRepository = departmentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var hotelId = _currentUserService.HotelId;

            return await _departmentRepository.Query()
                .Where(d => d.HotelId == hotelId)
                .Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    HotelId = d.HotelId,
                    DepartmentName = d.DepartmentName,
                    DepartmentCode = d.DepartmentCode,
                    IsActive = d.IsActive,
                    BudgetedHeadcount = d.BudgetedHeadcount,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<DepartmentDto> GetDepartmentByIdAsync(int id)
        {
            var department = await _departmentRepository.GetAsync<DepartmentDto>(id)
                ?? throw new NotFoundException($"Department '{id}' not found.");

            if (department.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            return department;
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            var currentHotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim in access token.");

            // 1. Explicitly prevent cross-hotel department creation
            // If a HotelId was passed in the request body and it doesn't match the JWT claim, reject it immediately.
            if (dto.HotelId != Guid.Empty && dto.HotelId != currentHotelId)
            {
                throw new ForbiddenException("You are not authorized to create departments for another hotel.");
            }

            // 2. Force the HotelId to match the current user's JWT claim
            dto.HotelId = currentHotelId;

            // 3. Save via repository
            return await _departmentRepository.AddAsync<CreateDepartmentDto, DepartmentDto>(dto);
        }

        public async Task UpdateDepartmentAsync(int id, BaseDepartmentDto dto)
        {
            var exists = await _departmentRepository.GetAsync(id)
                ?? throw new NotFoundException($"Department '{id}' not found.");

            if (exists.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            await _departmentRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var exists = await _departmentRepository.GetAsync(id)
                ?? throw new NotFoundException($"Department '{id}' not found.");

            if (exists.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            await _departmentRepository.DeleteAsync(id);
        }
    }
}
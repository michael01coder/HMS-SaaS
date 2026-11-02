using HMS.StaffService.Common.Exceptions;
using HMS.StaffService.Common.Service;
using HMS.StaffService.Domain.Data;
using HMS.StaffService.DTO.Staff;
using HMS.StaffService.Repository.Contracts;
using HMS.StaffService.Service.Contracts;
using HMS.StaffService.Service.External;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.StaffService.Service
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAuthServiceClient _authClient;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<StaffService> _logger;

        public StaffService(
            IStaffRepository staffRepository,
            IDepartmentRepository departmentRepository,
            IAuthServiceClient authClient,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<StaffService> logger)
        {
            _staffRepository = staffRepository;
            _departmentRepository = departmentRepository;
            _authClient = authClient;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<StaffDto>> GetAllStaffAsync()
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");
            var staffId = _currentUserService.StaffId;
            var role = _currentUserService.Role;

            var query = _staffRepository.Query().Where(s => s.HotelId == hotelId);

            if (role == "Manager")
            {
                var manager = await _staffRepository.Query().FirstOrDefaultAsync(s => s.StaffId == staffId);
                if (manager == null) return Enumerable.Empty<StaffDto>();
                query = query.Where(s => s.DepartmentId == manager.DepartmentId);
            }
            else if (role == "Staff")
            {
                query = query.Where(s => s.StaffId == staffId);
            }

            var list = await query.ProjectToType<StaffDto>().ToListAsync();
            list.ForEach(MaskSalary);
            return list;
        }

        public async Task<StaffDto> GetStaffByIdAsync(Guid id)
        {
            var targetStaff = await _staffRepository.GetAsync(id)
                ?? throw new NotFoundException($"Staff '{id}' was not found.");

            if (targetStaff.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied. Cross-tenant access restricted.");

            var role = _currentUserService.Role;
            if (role == "Staff" && targetStaff.StaffId != _currentUserService.StaffId)
                throw new ForbiddenException("You can only view your own profile.");

            if (role == "Manager" && targetStaff.StaffId != _currentUserService.StaffId)
            {
                var me = await _staffRepository.GetAsync(_currentUserService.StaffId ?? Guid.Empty);
                if (me == null || me.DepartmentId != targetStaff.DepartmentId)
                    throw new ForbiddenException("You can only view staff in your own department.");
            }

            var dto = _mapper.Map<StaffDto>(targetStaff);
            MaskSalary(dto);
            return dto;
        }

        private void MaskSalary(StaffDto dto)
        {
            var role = _currentUserService.Role;
            if ((role == "Manager" || role == "Staff") && dto.StaffId != _currentUserService.StaffId)
                dto.Salary = 0;
        }

        private async Task EnsureDepartmentInHotelAsync(int departmentId, Guid hotelId)
        {
            var department = await _departmentRepository.GetAsync(departmentId)
                ?? throw new BadRequestException("Department does not exist.");
            if (department.HotelId != hotelId)
                throw new BadRequestException("Department belongs to a different hotel.");
        }

        private async Task EnsureManagerInHotelAsync(Guid managerId, Guid hotelId, Guid selfId)
        {
            if (managerId == selfId) throw new BadRequestException("A staff member cannot manage themselves.");
            var manager = await _staffRepository.GetAsync(managerId)
                ?? throw new BadRequestException("Manager does not exist.");
            if (manager.HotelId != hotelId)
                throw new BadRequestException("Manager belongs to a different hotel.");
        }

        public async Task<StaffDto> CreateStaffAsync(CreateStaffDto dto)
        {
            dto.HotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            await EnsureDepartmentInHotelAsync(dto.DepartmentId, dto.HotelId);
            if (dto.ManagerId.HasValue)
                await EnsureManagerInHotelAsync(dto.ManagerId.Value, dto.HotelId, Guid.Empty);

            return await _staffRepository.AddAsync<CreateStaffDto, StaffDto>(dto);
        }

        public async Task<StaffDto> CreateStaffFromAuthAsync(InternalCreateStaffDto dto)
        {
            if (await _staffRepository.ExistsAsync(s => s.StaffId == dto.StaffId))
                throw new BadRequestException($"Staff '{dto.StaffId}' already exists — this StaffId was already registered.");

            var department = await _departmentRepository.GetAsync(dto.DepartmentId)
                ?? throw new BadRequestException("Department does not exist.");
            if (department.HotelId != dto.HotelId)
                throw new BadRequestException("Department belongs to a different hotel.");

            var entity = new Staff
            {
                StaffId = dto.StaffId, 
                HotelId = dto.HotelId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DepartmentId = dto.DepartmentId,
                EmployeeCode = $"EMP-{dto.StaffId.ToString("N")[..8].ToUpperInvariant()}",
                Designation = string.Empty,
                Role = dto.Role,
                EmploymentType = "Full-Time",
                JoiningDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Salary = 0,
                IsActive = true,
            };

            await _staffRepository.AddAsync(entity);

            var result = _mapper.Map<StaffDto>(entity);
            result.DepartmentName = department.DepartmentName;
            return result;
        }

        public async Task UpdateStaffAsync(Guid id, UpdateStaffDto dto)
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            var existing = await _staffRepository.GetAsync(id)
                ?? throw new NotFoundException($"Staff '{id}' not found.");

            if (existing.HotelId != hotelId)
                throw new ForbiddenException("Access denied.");

            if (dto.DepartmentId.HasValue && dto.DepartmentId.Value != existing.DepartmentId)
                await EnsureDepartmentInHotelAsync(dto.DepartmentId.Value, hotelId);

            if (dto.ManagerId.HasValue && dto.ManagerId != existing.ManagerId)
                await EnsureManagerInHotelAsync(dto.ManagerId.Value, hotelId, id);

            if (dto.IsActive.HasValue && dto.IsActive.Value != existing.IsActive)
                await _authClient.SetLoginActiveAsync(id, dto.IsActive.Value);

            await _staffRepository.UpdateAsync(id, dto);

            if (dto.DepartmentId.HasValue && dto.DepartmentId.Value != existing.DepartmentId)
            {
                var department = await _departmentRepository.GetAsync(dto.DepartmentId.Value);
                await _authClient.SetDepartmentAsync(id, department?.DepartmentName);
            }
        }

        public async Task DeleteStaffAsync(Guid id)
        {
            var existing = await _staffRepository.GetAsync(id)
                ?? throw new NotFoundException($"Staff '{id}' not found.");

            if (existing.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            if (id == _currentUserService.StaffId)
                throw new BadRequestException("You cannot delete your own record.");

            await _authClient.SetLoginActiveAsync(id, false);
            await _staffRepository.DeleteAsync(id);
        }
    }
}
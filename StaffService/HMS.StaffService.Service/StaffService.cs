using HMS.StaffService.Common.Exceptions;
using HMS.StaffService.Common.Service;
using HMS.StaffService.Domain.Data;
using HMS.StaffService.DTO.Staff;
using HMS.StaffService.Repository.Contracts;
using HMS.StaffService.Service.Contracts;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.StaffService.Service
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<StaffService> _logger;

        public StaffService(
            IStaffRepository staffRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<StaffService> logger)
        {
            _staffRepository = staffRepository;
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

            return await query.ProjectToType<StaffDto>().ToListAsync();
        }

        public async Task<StaffDto> GetStaffByIdAsync(Guid id)
        {
            var targetStaff = await _staffRepository.GetAsync(id)
                ?? throw new NotFoundException($"Staff '{id}' was not found.");

            if (targetStaff.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied. Cross-tenant access restricted.");

            return _mapper.Map<StaffDto>(targetStaff);
        }

        public async Task<StaffDto> CreateStaffAsync(CreateStaffDto dto)
        {
            dto.HotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            return await _staffRepository.AddAsync<CreateStaffDto, StaffDto>(dto);
        }

        public async Task<StaffDto> CreateStaffFromAuthAsync(InternalCreateStaffDto dto)
        {
            if (await _staffRepository.ExistsAsync(s => s.StaffId == dto.StaffId))
                throw new BadRequestException($"Staff '{dto.StaffId}' already exists — this StaffId was already registered.");

            var entity = new Staff
            {
                StaffId = dto.StaffId, // pinned — must match the id already on the Identity user's JWT
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

            return _mapper.Map<StaffDto>(entity);
        }

        public async Task UpdateStaffAsync(Guid id, UpdateStaffDto dto)
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            var existing = await _staffRepository.GetAsync(id)
                ?? throw new NotFoundException($"Staff '{id}' not found.");

            if (existing.HotelId != hotelId)
                throw new ForbiddenException("Access denied.");

            await _staffRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteStaffAsync(Guid id)
        {
            var existing = await _staffRepository.GetAsync(id)
                ?? throw new NotFoundException($"Staff '{id}' not found.");

            if (existing.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            await _staffRepository.DeleteAsync(id);
        }
    }
}
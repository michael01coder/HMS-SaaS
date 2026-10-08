using HMS.AttendanceService.Common.Exceptions;
using HMS.AttendanceService.Common.Service;
using HMS.AttendanceService.Domain.Data;
using HMS.AttendanceService.DTO.Leave;
using HMS.AttendanceService.Repository.Contracts;
using HMS.AttendanceService.Service.Contracts;
using MapsterMapper;

namespace HMS.AttendanceService.Service
{
    public class LeaveApplicationService : ILeaveApplicationService
    {
        private readonly ILeaveApplicationRepository _leaveRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public LeaveApplicationService(
            ILeaveApplicationRepository leaveRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _leaveRepository = leaveRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<LeaveApplicationDto> ApplyLeaveAsync(CreateLeaveApplicationDto dto)
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");
            var tenantId = _currentUserService.TenantId
                ?? throw new ForbiddenException("Missing TenantId claim.");

            int requestedDays = (dto.EndDate.ToDateTime(TimeOnly.MinValue) - dto.StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1;

            // Check remaining balance
            var balance = await GetStaffLeaveBalanceAsync(dto.StaffId, dto.LeaveTypeName, dto.StartDate.Year);

            if (requestedDays > balance.RemainingDays)
            {
                throw new BadRequestException(
                    $"Insufficient leave balance. You have {balance.RemainingDays} days remaining for {dto.LeaveTypeName}, but requested {requestedDays} days.");
            }

            if (dto.EndDate < dto.StartDate)
                throw new BadRequestException("EndDate cannot be earlier than StartDate.");

            var leave = _mapper.Map<LeaveApplication>(dto);
            leave.LeaveApplicationId = Guid.NewGuid();
            leave.TenantId = tenantId;
            leave.HotelId = hotelId;
            leave.Status = "Pending";
            leave.AppliedOn = DateOnly.FromDateTime(DateTime.UtcNow);
            leave.CreatedAt = DateTime.UtcNow;

            if (leave.TotalDays <= 0)
            {
                leave.TotalDays = (dto.EndDate.ToDateTime(TimeOnly.MinValue) - dto.StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1;
            }

            await _leaveRepository.AddAsync(leave);
            return _mapper.Map<LeaveApplicationDto>(leave);
        }

        public async Task<LeaveApplicationDto> GetByIdAsync(Guid id)
        {
            var leave = await _leaveRepository.GetAsync(id)
                ?? throw new NotFoundException($"Leave application with ID '{id}' was not found.");

            return _mapper.Map<LeaveApplicationDto>(leave);
        }

        public async Task<IEnumerable<LeaveApplicationDto>> GetByHotelAsync()
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            var list = await _leaveRepository.GetByHotelIdAsync(hotelId);
            return _mapper.Map<List<LeaveApplicationDto>>(list);
        }

        public async Task<IEnumerable<LeaveApplicationDto>> GetByStaffAsync(Guid staffId)
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            var list = await _leaveRepository.GetByStaffIdAsync(hotelId, staffId);
            return _mapper.Map<List<LeaveApplicationDto>>(list);
        }

        public async Task<IEnumerable<LeaveApplicationDto>> GetPendingForHotelAsync()
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            var list = await _leaveRepository.GetPendingByHotelIdAsync(hotelId);
            return _mapper.Map<List<LeaveApplicationDto>>(list);
        }

        public async Task<LeaveApplicationDto> UpdateStatusAsync(Guid id, UpdateLeaveStatusDto dto)
        {
            var leave = await _leaveRepository.GetAsync(id)
                ?? throw new NotFoundException($"Leave application with ID '{id}' was not found.");

            leave.Status = dto.Status;
            leave.ApprovedByStaffId = dto.ApprovedByStaffId ?? _currentUserService.StaffId;
            leave.ApprovalDate = DateOnly.FromDateTime(DateTime.UtcNow);
            leave.UpdatedAt = DateTime.UtcNow;

            await _leaveRepository.UpdateAsync(id, leave);
            return _mapper.Map<LeaveApplicationDto>(leave);
        }

        public async Task DeleteAsync(Guid id)
        {
            if (!await _leaveRepository.ExistsAsync(l => l.LeaveApplicationId == id))
                throw new NotFoundException($"Leave application with ID '{id}' was not found.");

            await _leaveRepository.DeleteAsync(id);
        }

        public async Task<LeaveBalanceDto> GetStaffLeaveBalanceAsync(Guid staffId, string leaveTypeName, int year)
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            var approvedLeaves = await _leaveRepository.GetApprovedLeavesForYearAsync(hotelId, staffId, leaveTypeName, year);

            int usedDays = approvedLeaves.Sum(l => l.TotalDays);
            int totalQuota = 12;

            return new LeaveBalanceDto
            {
                StaffId = staffId,
                LeaveTypeName = leaveTypeName,
                Year = year,
                TotalQuota = totalQuota,
                UsedDays = usedDays,
                RemainingDays = Math.Max(0, totalQuota - usedDays)
            };
        }
    }
}
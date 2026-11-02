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

            if (_currentUserService.Role != "HR" && dto.StaffId != _currentUserService.StaffId)
                throw new ForbiddenException("You can only apply for leave for yourself.");

            if (dto.EndDate < dto.StartDate)
                throw new BadRequestException("EndDate cannot be earlier than StartDate.");

            int requestedDays = (dto.EndDate.ToDateTime(TimeOnly.MinValue) - dto.StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1;

            if (await _leaveRepository.HasOverlapAsync(hotelId, dto.StaffId, dto.StartDate, dto.EndDate))
                throw new BadRequestException("You already have a leave request covering these dates.");

            var balance = await GetStaffLeaveBalanceAsync(dto.StaffId, dto.LeaveTypeName, dto.StartDate.Year);

            if (requestedDays > balance.RemainingDays)
            {
                throw new BadRequestException(
                    $"Insufficient leave balance. You have {balance.RemainingDays} days remaining for {dto.LeaveTypeName}, but requested {requestedDays} days.");
            }

            var leave = _mapper.Map<LeaveApplication>(dto);
            leave.RequesterRole = _currentUserService.StaffId == dto.StaffId ? _currentUserService.Role ?? "Staff" : "Staff";
            leave.Department = _currentUserService.StaffId == dto.StaffId ? _currentUserService.Department : null;
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

            if (leave.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            if (_currentUserService.Role is "Staff" && leave.StaffId != _currentUserService.StaffId)
                throw new ForbiddenException("You can only view your own leave.");

            return _mapper.Map<LeaveApplicationDto>(leave);
        }

        public async Task<IEnumerable<LeaveApplicationDto>> GetByHotelAsync()
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            var list = await _leaveRepository.GetByHotelIdAsync(hotelId);
            return _mapper.Map<List<LeaveApplicationDto>>(ScopeForViewer(list));
        }

        public async Task<IEnumerable<LeaveApplicationDto>> GetByStaffAsync(Guid staffId)
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            if (_currentUserService.Role is "Staff" && staffId != _currentUserService.StaffId)
                throw new ForbiddenException("You can only view your own leave.");

            var list = await _leaveRepository.GetByStaffIdAsync(hotelId, staffId);
            return _mapper.Map<List<LeaveApplicationDto>>(list);
        }

        public async Task<IEnumerable<LeaveApplicationDto>> GetPendingForHotelAsync()
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            var list = await _leaveRepository.GetPendingByHotelIdAsync(hotelId);
            return _mapper.Map<List<LeaveApplicationDto>>(ScopeForViewer(list).Where(CanDecide).ToList());
        }

        public async Task<LeaveApplicationDto> UpdateStatusAsync(Guid id, UpdateLeaveStatusDto dto)
        {
            var leave = await _leaveRepository.GetAsync(id)
                ?? throw new NotFoundException($"Leave application with ID '{id}' was not found.");

            if (leave.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            if (leave.Status != "Pending")
                throw new BadRequestException($"This request is already {leave.Status}.");

            if (dto.Status is not ("Approved" or "Rejected"))
                throw new BadRequestException("Status must be Approved or Rejected.");

            if (leave.StaffId == _currentUserService.StaffId)
                throw new ForbiddenException("You cannot decide your own leave request.");

            if (!CanDecide(leave))
                throw new ForbiddenException("Your role cannot decide this leave request.");

            leave.Status = dto.Status;
            leave.ApprovedByStaffId = _currentUserService.StaffId;
            leave.ApprovalDate = DateOnly.FromDateTime(DateTime.UtcNow);
            leave.UpdatedAt = DateTime.UtcNow;

            await _leaveRepository.UpdateAsync(id, leave);
            return _mapper.Map<LeaveApplicationDto>(leave);
        }

        private bool CanDecide(LeaveApplication leave)
        {
            var viewer = _currentUserService.Role;
            return leave.RequesterRole switch
            {
                "HR" => viewer == "HotelOwner",
                "Manager" => viewer is "HR" or "HotelOwner",
                _ => viewer is "HR" or "HotelOwner"
                     || (viewer == "Manager" && !string.IsNullOrEmpty(leave.Department)
                         && string.Equals(leave.Department, _currentUserService.Department, StringComparison.OrdinalIgnoreCase))
            };
        }

        private List<LeaveApplication> ScopeForViewer(IEnumerable<LeaveApplication> list)
        {
            if (_currentUserService.Role != "Manager") return list.ToList();
            return list.Where(l => l.StaffId == _currentUserService.StaffId || CanDecide(l)).ToList();
        }

        public async Task DeleteAsync(Guid id)
        {
            var leave = await _leaveRepository.GetAsync(id)
                ?? throw new NotFoundException($"Leave application with ID '{id}' was not found.");

            if (leave.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

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
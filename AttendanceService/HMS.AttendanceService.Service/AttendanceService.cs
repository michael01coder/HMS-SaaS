using HMS.AttendanceService.Common.Exceptions;
using HMS.AttendanceService.Common.Service;
using HMS.AttendanceService.Domain.Data;
using HMS.AttendanceService.DTO.Attendance;
using HMS.AttendanceService.Repository.Contracts;
using HMS.AttendanceService.Service.Contracts;
using MapsterMapper;

namespace HMS.AttendanceService.Service
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _attendanceRepository = attendanceRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<AttendanceDto> ClockInAsync(ClockInDto dto)
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");
            var tenantId = _currentUserService.TenantId
                ?? throw new ForbiddenException("Missing TenantId claim.");

            EnsureOwnRecord(dto.StaffId);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var existing = await _attendanceRepository.GetTodayRecordAsync(hotelId, dto.StaffId, today);
            if (existing != null)
                throw new BadRequestException("Staff member has already clocked in for today.");

            var attendance = new Attendance
            {
                AttendanceId = Guid.NewGuid(),
                TenantId = tenantId,
                HotelId = hotelId,
                StaffId = dto.StaffId,
                AttendanceDate = today,
                ClockInTime = DateTime.UtcNow,
                Status = "Present",
                Remarks = dto.Remarks
            };

            await _attendanceRepository.AddAsync(attendance);
            return _mapper.Map<AttendanceDto>(attendance);
        }

        public async Task<AttendanceDto> ClockOutAsync(ClockOutDto dto)
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            EnsureOwnRecord(dto.StaffId);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var attendance = await _attendanceRepository.GetTodayRecordAsync(hotelId, dto.StaffId, today)
                ?? throw new NotFoundException("No clock-in record found for today.");

            if (attendance.ClockOutTime.HasValue)
                throw new BadRequestException("Staff member has already clocked out for today.");

            attendance.ClockOutTime = DateTime.UtcNow;
            var duration = attendance.ClockOutTime.Value - attendance.ClockInTime;
            attendance.TotalHours = Math.Round((decimal)duration.TotalHours, 2);
            attendance.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(dto.Remarks))
                attendance.Remarks = string.IsNullOrEmpty(attendance.Remarks)
                    ? dto.Remarks
                    : $"{attendance.Remarks} | Out: {dto.Remarks}";

            await _attendanceRepository.UpdateAsync(attendance.AttendanceId, attendance);
            return _mapper.Map<AttendanceDto>(attendance);
        }

        public async Task<IEnumerable<AttendanceDto>> GetDailyAttendanceAsync(DateOnly date)
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            var list = await _attendanceRepository.GetHotelDailyAttendanceAsync(hotelId, date);
            return _mapper.Map<List<AttendanceDto>>(list);
        }

        public async Task<IEnumerable<AttendanceDto>> GetStaffHistoryAsync(Guid staffId, DateOnly startDate, DateOnly endDate)
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            if (_currentUserService.Role is "Staff" && staffId != _currentUserService.StaffId)
                throw new ForbiddenException("You can only view your own attendance.");

            var list = await _attendanceRepository.GetStaffHistoryAsync(hotelId, staffId, startDate, endDate);
            return _mapper.Map<List<AttendanceDto>>(list);
        }

        public async Task<AttendanceDto> ManualCreateAsync(CreateAttendanceDto dto)
        {
            dto.HotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");
            dto.TenantId = _currentUserService.TenantId
                ?? throw new ForbiddenException("Missing TenantId claim.");

            return await _attendanceRepository.AddAsync<CreateAttendanceDto, AttendanceDto>(dto);
        }

        private void EnsureOwnRecord(Guid staffId)
        {
            if (staffId != _currentUserService.StaffId)
                throw new ForbiddenException("You can only clock in or out for yourself.");
        }

        public async Task UpdateAsync(Guid id, UpdateAttendanceDto dto)
        {
            var record = await _attendanceRepository.GetAsync(id)
                ?? throw new NotFoundException($"Attendance record with ID '{id}' was not found.");

            if (record.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            await _attendanceRepository.UpdateAsync(id, dto);
        }
    }
}
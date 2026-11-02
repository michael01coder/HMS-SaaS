using HMS.AttendanceService.Common.Exceptions;
using HMS.AttendanceService.Common.Service;
using HMS.AttendanceService.DTO.Anomaly;
using HMS.AttendanceService.Repository.Contracts;
using HMS.AttendanceService.Service.Contracts;
using MapsterMapper;

namespace HMS.AttendanceService.Service
{
    public class AttendanceAnomalyService : IAttendanceAnomalyService
    {
        private readonly IAttendanceAnomalyRepository _anomalyRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public AttendanceAnomalyService(
            IAttendanceAnomalyRepository anomalyRepository,
            IAttendanceRepository attendanceRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _anomalyRepository = anomalyRepository;
            _attendanceRepository = attendanceRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<AttendanceAnomalyDto> CreateAnomalyAsync(CreateAttendanceAnomalyDto dto)
        {
            var attendance = await _attendanceRepository.GetAsync(dto.AttendanceId)
                ?? throw new NotFoundException($"Attendance record with ID '{dto.AttendanceId}' was not found.");

            if (attendance.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            return await _anomalyRepository.AddAsync<CreateAttendanceAnomalyDto, AttendanceAnomalyDto>(dto);
        }

        public async Task<IEnumerable<AttendanceAnomalyDto>> GetAnomaliesByAttendanceIdAsync(Guid attendanceId)
        {
            await EnsureAttendanceInHotelAsync(attendanceId);
            var anomalies = await _anomalyRepository.GetByAttendanceIdAsync(attendanceId);
            return _mapper.Map<List<AttendanceAnomalyDto>>(anomalies);
        }

        public async Task<IEnumerable<AttendanceAnomalyDto>> GetAnomaliesByStaffIdAsync(Guid staffId)
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            var anomalies = await _anomalyRepository.GetByStaffIdAsync(staffId);
            var scoped = anomalies.Where(a => a.Attendance != null && a.Attendance.HotelId == hotelId);
            return _mapper.Map<List<AttendanceAnomalyDto>>(scoped.ToList());
        }

        public async Task<IEnumerable<AttendanceAnomalyDto>> GetPendingAnomaliesForHotelAsync()
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            var anomalies = await _anomalyRepository.GetPendingAnomaliesByHotelAsync(hotelId);
            return _mapper.Map<List<AttendanceAnomalyDto>>(anomalies);
        }

        public async Task UpdateAnomalyAsync(Guid id, UpdateAttendanceAnomalyDto dto)
        {
            await EnsureAnomalyInHotelAsync(id);
            await _anomalyRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteAnomalyAsync(Guid id)
        {
            await EnsureAnomalyInHotelAsync(id);
            await _anomalyRepository.DeleteAsync(id);
        }

        private async Task EnsureAttendanceInHotelAsync(Guid attendanceId)
        {
            var attendance = await _attendanceRepository.GetAsync(attendanceId)
                ?? throw new NotFoundException($"Attendance record with ID '{attendanceId}' was not found.");
            if (attendance.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");
        }

        private async Task EnsureAnomalyInHotelAsync(Guid id)
        {
            var anomaly = await _anomalyRepository.GetAsync(id)
                ?? throw new NotFoundException($"Attendance anomaly with ID '{id}' was not found.");
            await EnsureAttendanceInHotelAsync(anomaly.AttendanceId);
        }
    }
}

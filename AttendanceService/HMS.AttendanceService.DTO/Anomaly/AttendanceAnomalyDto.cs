namespace HMS.AttendanceService.DTO.Anomaly
{
    public class AttendanceAnomalyDto : BaseAttendanceAnomalyDto
    {
        public Guid AnomalyId { get; set; }
        public DateTime ReportedAt { get; set; }
    }
}
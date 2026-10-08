using HMS.AttendanceService.Domain.Data;
using HMS.AttendanceService.DTO.Attendance;
using HMS.AttendanceService.DTO.Anomaly;
using Mapster;

namespace HMS.AttendanceService.Api.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            config.NewConfig<Attendance, AttendanceDto>();

            config.NewConfig<CreateAttendanceDto, Attendance>()
                .Ignore(dest => dest.AttendanceId)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Anomalies);

            config.NewConfig<UpdateAttendanceDto, Attendance>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.AttendanceId)
                .Ignore(dest => dest.TenantId)
                .Ignore(dest => dest.HotelId)
                .Ignore(dest => dest.StaffId)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.Anomalies);

            // ----------------------------------------------------
            // Attendance Anomaly Mappings
            // ----------------------------------------------------
            config.NewConfig<AttendanceAnomaly, AttendanceAnomalyDto>();

            config.NewConfig<CreateAttendanceAnomalyDto, AttendanceAnomaly>()
                .Ignore(dest => dest.AnomalyId)
                .Ignore(dest => dest.ReportedAt)
                .Ignore(dest => dest.Attendance);

            config.NewConfig<UpdateAttendanceAnomalyDto, AttendanceAnomaly>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.AnomalyId)
                .Ignore(dest => dest.AttendanceId)
                .Ignore(dest => dest.StaffId)
                .Ignore(dest => dest.ReportedAt)
                .Ignore(dest => dest.Attendance);
        }
    }
}
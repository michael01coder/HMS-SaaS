using HMS.AttendanceService.Domain.Data;
using HMS.AttendanceService.DTO.Attendance;
using HMS.AttendanceService.DTO.Anomaly;
using HMS.AttendanceService.DTO.Leave;
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

            // ----------------------------------------------------
            // Leave Application Mappings
            // ----------------------------------------------------
            config.NewConfig<LeaveApplication, LeaveApplicationDto>();

            config.NewConfig<CreateLeaveApplicationDto, LeaveApplication>()
                .Ignore(dest => dest.LeaveApplicationId)
                .Ignore(dest => dest.TenantId)
                .Ignore(dest => dest.HotelId)
                .Ignore(dest => dest.Status)
                .Ignore(dest => dest.AppliedOn)
                .Ignore(dest => dest.ApprovedByStaffId)
                .Ignore(dest => dest.ApprovalDate)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt);

            config.NewConfig<UpdateLeaveStatusDto, LeaveApplication>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.LeaveApplicationId)
                .Ignore(dest => dest.TenantId)
                .Ignore(dest => dest.HotelId)
                .Ignore(dest => dest.StaffId)
                .Ignore(dest => dest.LeaveTypeName)
                .Ignore(dest => dest.StartDate)
                .Ignore(dest => dest.EndDate)
                .Ignore(dest => dest.TotalDays)
                .Ignore(dest => dest.Reason)
                .Ignore(dest => dest.AppliedOn)
                .Ignore(dest => dest.DefaultDaysPerYear)
                .Ignore(dest => dest.CreatedAt);
        }
    }
}
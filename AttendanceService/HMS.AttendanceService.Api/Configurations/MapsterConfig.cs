using HMS.AttendanceService.Domain.Data;
using HMS.AttendanceService.DTO.Anomaly;
using HMS.AttendanceService.DTO.Attendance;
using HMS.AttendanceService.DTO.Leave;
using HMS.AttendanceService.DTO.Shift;
using Mapster;

namespace HMS.AttendanceService.Api.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            // Attendance Mappings
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

            // Attendance Anomaly Mappings
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

            // Leave Application Mappings
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

            // Shift Master Mappings
            config.NewConfig<Shift, ShiftDto>();

            config.NewConfig<CreateShiftDto, Shift>()
                .Ignore(dest => dest.ShiftId)
                .Ignore(dest => dest.TenantId)
                .Ignore(dest => dest.HotelId)
                .Ignore(dest => dest.IsActive)
                .Ignore(dest => dest.IsDeleted)
                .Ignore(dest => dest.CreatedAt);

            config.NewConfig<UpdateShiftDto, Shift>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.ShiftId)
                .Ignore(dest => dest.TenantId)
                .Ignore(dest => dest.HotelId)
                .Ignore(dest => dest.IsDeleted)
                .Ignore(dest => dest.CreatedAt);

            // Shift Allocation Mappings
            config.NewConfig<ShiftAllocation, ShiftAllocationDto>();

            config.NewConfig<CreateShiftAllocationDto, ShiftAllocation>()
                .Ignore(dest => dest.AllocationId)
                .Ignore(dest => dest.TenantId)
                .Ignore(dest => dest.HotelId)
                .Ignore(dest => dest.IsDeleted)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.Shift);

            // Shift Swap Request Mappings
            config.NewConfig<ShiftSwapRequest, ShiftSwapRequestDto>();

            config.NewConfig<CreateShiftSwapRequestDto, ShiftSwapRequest>()
                .Ignore(dest => dest.SwapRequestId)
                .Ignore(dest => dest.TenantId)
                .Ignore(dest => dest.HotelId)
                .Ignore(dest => dest.Status)
                .Ignore(dest => dest.ApprovedByStaffId)
                .Ignore(dest => dest.RequestedAt)
                .Ignore(dest => dest.RespondedAt)
                .Ignore(dest => dest.IsDeleted)
                .Ignore(dest => dest.RequesterAllocation)
                .Ignore(dest => dest.TargetAllocation);

            config.NewConfig<UpdateShiftSwapStatusDto, ShiftSwapRequest>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.SwapRequestId)
                .Ignore(dest => dest.TenantId)
                .Ignore(dest => dest.HotelId)
                .Ignore(dest => dest.RequesterStaffId)
                .Ignore(dest => dest.RequesterAllocationId)
                .Ignore(dest => dest.TargetStaffId)
                .Ignore(dest => dest.TargetAllocationId)
                .Ignore(dest => dest.Reason)
                .Ignore(dest => dest.RequestedAt)
                .Ignore(dest => dest.RespondedAt)
                .Ignore(dest => dest.IsDeleted)
                .Ignore(dest => dest.RequesterAllocation)
                .Ignore(dest => dest.TargetAllocation);
        }
    }
}
using HMS.AttendanceService.DTO.Shift;

namespace HMS.AttendanceService.Service.Contracts
{
    public interface IShiftService
    {
        Task<ShiftDto> CreateShiftAsync(CreateShiftDto dto);
        Task<IEnumerable<ShiftDto>> GetHotelShiftsAsync();
        Task UpdateShiftAsync(Guid id, UpdateShiftDto dto);

        Task<ShiftAllocationDto> AllocateShiftAsync(CreateShiftAllocationDto dto);
        Task<IEnumerable<ShiftAllocationDto>> BulkAllocateShiftsAsync(BulkShiftAllocationDto dto);
        Task<IEnumerable<ShiftAllocationDto>> GetHotelAllocationsAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<ShiftAllocationDto>> GetStaffAllocationsAsync(Guid staffId, DateOnly startDate, DateOnly endDate);

        Task<ShiftSwapRequestDto> CreateSwapRequestAsync(CreateShiftSwapRequestDto dto);
        Task<IEnumerable<ShiftSwapRequestDto>> GetPendingSwapsAsync();
        Task<IEnumerable<ShiftSwapRequestDto>> GetStaffSwapsAsync(Guid staffId);
        Task<ShiftSwapRequestDto> UpdateSwapStatusAsync(Guid swapRequestId, UpdateShiftSwapStatusDto dto);
    }
}
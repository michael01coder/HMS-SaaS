using HMS.AttendanceService.Common.Exceptions;
using HMS.AttendanceService.Common.Service;
using HMS.AttendanceService.Domain.Data;
using HMS.AttendanceService.DTO.Shift;
using HMS.AttendanceService.Repository.Contracts;
using HMS.AttendanceService.Service.Contracts;
using MapsterMapper;

namespace HMS.AttendanceService.Service
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepo;
        private readonly IShiftAllocationRepository _allocationRepo;
        private readonly IShiftSwapRequestRepository _swapRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public ShiftService(
            IShiftRepository shiftRepo,
            IShiftAllocationRepository allocationRepo,
            IShiftSwapRequestRepository swapRepo,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _shiftRepo = shiftRepo;
            _allocationRepo = allocationRepo;
            _swapRepo = swapRepo;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ShiftDto> CreateShiftAsync(CreateShiftDto dto)
        {
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var tenantId = _currentUserService.TenantId ?? throw new ForbiddenException("Missing TenantId.");

            var shift = _mapper.Map<Shift>(dto);
            shift.ShiftId = Guid.NewGuid();
            shift.TenantId = tenantId;
            shift.HotelId = hotelId;

            await _shiftRepo.AddAsync(shift);
            return _mapper.Map<ShiftDto>(shift);
        }

        public async Task<IEnumerable<ShiftDto>> GetHotelShiftsAsync()
        {
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var list = await _shiftRepo.GetActiveShiftsByHotelAsync(hotelId);
            return _mapper.Map<List<ShiftDto>>(list);
        }

        public async Task UpdateShiftAsync(Guid id, UpdateShiftDto dto)
        {
            if (!await _shiftRepo.ExistsAsync(s => s.ShiftId == id))
                throw new NotFoundException($"Shift with ID '{id}' was not found.");

            await _shiftRepo.UpdateAsync(id, dto);
        }

        public async Task<ShiftAllocationDto> AllocateShiftAsync(CreateShiftAllocationDto dto)
        {
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var tenantId = _currentUserService.TenantId ?? throw new ForbiddenException("Missing TenantId.");

            if (await _allocationRepo.HasConflictAsync(hotelId, dto.StaffId, dto.WorkDate))
                throw new BadRequestException($"Staff member already has a shift assigned on {dto.WorkDate}.");

            var allocation = _mapper.Map<ShiftAllocation>(dto);
            allocation.AllocationId = Guid.NewGuid();
            allocation.TenantId = tenantId;
            allocation.HotelId = hotelId;

            await _allocationRepo.AddAsync(allocation);
            return _mapper.Map<ShiftAllocationDto>(allocation);
        }

        public async Task<IEnumerable<ShiftAllocationDto>> BulkAllocateShiftsAsync(BulkShiftAllocationDto dto)
        {
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var tenantId = _currentUserService.TenantId ?? throw new ForbiddenException("Missing TenantId.");

            var allocations = new List<ShiftAllocation>();

            for (var date = dto.StartDate; date <= dto.EndDate; date = date.AddDays(1))
            {
                foreach (var staffId in dto.StaffIds)
                {
                    if (!await _allocationRepo.HasConflictAsync(hotelId, staffId, date))
                    {
                        allocations.Add(new ShiftAllocation
                        {
                            AllocationId = Guid.NewGuid(),
                            TenantId = tenantId,
                            HotelId = hotelId,
                            StaffId = staffId,
                            ShiftId = dto.ShiftId,
                            WorkDate = date,
                            AllocationType = dto.AllocationType
                        });
                    }
                }
            }

            foreach (var item in allocations)
            {
                await _allocationRepo.AddAsync(item);
            }

            return _mapper.Map<List<ShiftAllocationDto>>(allocations);
        }

        public async Task<IEnumerable<ShiftAllocationDto>> GetHotelAllocationsAsync(DateOnly startDate, DateOnly endDate)
        {
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var list = await _allocationRepo.GetByHotelAndDateRangeAsync(hotelId, startDate, endDate);
            return _mapper.Map<List<ShiftAllocationDto>>(list);
        }

        public async Task<IEnumerable<ShiftAllocationDto>> GetStaffAllocationsAsync(Guid staffId, DateOnly startDate, DateOnly endDate)
        {
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var list = await _allocationRepo.GetByStaffAndDateRangeAsync(hotelId, staffId, startDate, endDate);
            return _mapper.Map<List<ShiftAllocationDto>>(list);
        }

        public async Task<ShiftSwapRequestDto> CreateSwapRequestAsync(CreateShiftSwapRequestDto dto)
        {
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var tenantId = _currentUserService.TenantId ?? throw new ForbiddenException("Missing TenantId.");

            var swap = _mapper.Map<ShiftSwapRequest>(dto);
            swap.SwapRequestId = Guid.NewGuid();
            swap.TenantId = tenantId;
            swap.HotelId = hotelId;
            swap.Status = "Pending";

            await _swapRepo.AddAsync(swap);
            return _mapper.Map<ShiftSwapRequestDto>(swap);
        }

        public async Task<IEnumerable<ShiftSwapRequestDto>> GetPendingSwapsAsync()
        {
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var list = await _swapRepo.GetPendingByHotelAsync(hotelId);
            return _mapper.Map<List<ShiftSwapRequestDto>>(list);
        }

        public async Task<IEnumerable<ShiftSwapRequestDto>> GetStaffSwapsAsync(Guid staffId)
        {
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var list = await _swapRepo.GetByStaffAsync(hotelId, staffId);
            return _mapper.Map<List<ShiftSwapRequestDto>>(list);
        }

        public async Task<ShiftSwapRequestDto> UpdateSwapStatusAsync(Guid swapRequestId, UpdateShiftSwapStatusDto dto)
        {
            var swap = await _swapRepo.GetAsync(swapRequestId)
                ?? throw new NotFoundException($"Swap request with ID '{swapRequestId}' was not found.");

            swap.Status = dto.Status;
            swap.ApprovedByStaffId = dto.ApprovedByStaffId ?? _currentUserService.StaffId;
            swap.RespondedAt = DateTime.UtcNow;

            // Execute actual staff assignment swap on approval
            if (dto.Status == "Approved")
            {
                var reqAlloc = await _allocationRepo.GetAsync(swap.RequesterAllocationId);
                if (reqAlloc != null)
                {
                    reqAlloc.StaffId = swap.TargetStaffId;
                    await _allocationRepo.UpdateAsync(reqAlloc.AllocationId, reqAlloc);
                }

                if (swap.TargetAllocationId.HasValue)
                {
                    var targetAlloc = await _allocationRepo.GetAsync(swap.TargetAllocationId.Value);
                    if (targetAlloc != null)
                    {
                        targetAlloc.StaffId = swap.RequesterStaffId;
                        await _allocationRepo.UpdateAsync(targetAlloc.AllocationId, targetAlloc);
                    }
                }
            }

            await _swapRepo.UpdateAsync(swapRequestId, swap);
            return _mapper.Map<ShiftSwapRequestDto>(swap);
        }
    }
}
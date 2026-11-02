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
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var shift = await _shiftRepo.GetAsync(id)
                ?? throw new NotFoundException($"Shift with ID '{id}' was not found.");

            if (shift.HotelId != hotelId)
                throw new ForbiddenException("Access denied.");

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
            EnsureSelfOrPrivileged(staffId);
            var list = await _allocationRepo.GetByStaffAndDateRangeAsync(hotelId, staffId, startDate, endDate);
            return _mapper.Map<List<ShiftAllocationDto>>(list);
        }

        public async Task<ShiftSwapRequestDto> CreateSwapRequestAsync(CreateShiftSwapRequestDto dto)
        {
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var tenantId = _currentUserService.TenantId ?? throw new ForbiddenException("Missing TenantId.");

            var role = _currentUserService.Role;
            if (role != "HR" && dto.RequesterStaffId != _currentUserService.StaffId)
                throw new ForbiddenException("You can only request a swap for your own shift.");

            var ownAllocation = await _allocationRepo.GetAsync(dto.RequesterAllocationId)
                ?? throw new NotFoundException("The shift allocation was not found.");
            if (ownAllocation.HotelId != hotelId || ownAllocation.StaffId != dto.RequesterStaffId)
                throw new BadRequestException("The allocation does not belong to the requester.");
            if (ownAllocation.WorkDate < DateOnly.FromDateTime(DateTime.UtcNow))
                throw new BadRequestException("Past shifts cannot be swapped.");

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
            EnsureSelfOrPrivileged(staffId);
            var list = await _swapRepo.GetByStaffAsync(hotelId, staffId);
            return _mapper.Map<List<ShiftSwapRequestDto>>(list);
        }

        public async Task<ShiftSwapRequestDto> UpdateSwapStatusAsync(Guid swapRequestId, UpdateShiftSwapStatusDto dto)
        {
            var hotelId = _currentUserService.HotelId ?? throw new ForbiddenException("Missing HotelId.");
            var swap = await _swapRepo.GetAsync(swapRequestId)
                ?? throw new NotFoundException($"Swap request with ID '{swapRequestId}' was not found.");

            if (swap.HotelId != hotelId)
                throw new ForbiddenException("Access denied.");

            if (swap.Status != "Pending")
                throw new BadRequestException($"This request is already {swap.Status}.");

            if (dto.Status is not ("Approved" or "Rejected"))
                throw new BadRequestException("Status must be Approved or Rejected.");

            var approverId = _currentUserService.StaffId ?? Guid.Empty;
            if (approverId == swap.RequesterStaffId)
                throw new ForbiddenException("You cannot decide your own swap request.");

            if (dto.Status == "Approved")
            {
                var targetStaffId = dto.TargetStaffId ?? (swap.TargetStaffId != Guid.Empty ? swap.TargetStaffId : (Guid?)null)
                    ?? throw new BadRequestException("Choose the staff member who will take this shift.");

                if (targetStaffId == swap.RequesterStaffId)
                    throw new BadRequestException("The target must be a different staff member.");

                var requesterAlloc = await _allocationRepo.GetAsync(swap.RequesterAllocationId)
                    ?? throw new NotFoundException("The requester's shift allocation no longer exists.");

                ShiftAllocation? targetAlloc = null;
                var targetAllocationId = dto.TargetAllocationId ?? swap.TargetAllocationId;
                if (targetAllocationId.HasValue)
                {
                    targetAlloc = await _allocationRepo.GetAsync(targetAllocationId.Value)
                        ?? throw new NotFoundException("The target shift allocation was not found.");
                    if (targetAlloc.HotelId != hotelId || targetAlloc.StaffId != targetStaffId)
                        throw new BadRequestException("The target allocation does not belong to the selected staff member.");
                }
                else if (await _allocationRepo.HasConflictAsync(hotelId, targetStaffId, requesterAlloc.WorkDate))
                {
                    throw new BadRequestException("The selected staff member already has a shift on that date.");
                }

                requesterAlloc.StaffId = targetStaffId;
                await _allocationRepo.UpdateAsync(requesterAlloc.AllocationId, requesterAlloc);

                if (targetAlloc != null)
                {
                    targetAlloc.StaffId = swap.RequesterStaffId;
                    await _allocationRepo.UpdateAsync(targetAlloc.AllocationId, targetAlloc);
                }

                swap.TargetStaffId = targetStaffId;
                swap.TargetAllocationId = targetAllocationId;
            }

            swap.Status = dto.Status;
            swap.ApprovedByStaffId = approverId;
            swap.RespondedAt = DateTime.UtcNow;

            await _swapRepo.UpdateAsync(swapRequestId, swap);
            return _mapper.Map<ShiftSwapRequestDto>(swap);
        }

        private void EnsureSelfOrPrivileged(Guid staffId)
        {
            if (_currentUserService.Role == "Staff" && staffId != _currentUserService.StaffId)
                throw new ForbiddenException("You can only view your own records.");
        }
    }
}

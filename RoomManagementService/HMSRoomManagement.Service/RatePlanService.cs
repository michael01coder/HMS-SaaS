using HMSRoomManagement.Common.Constants;
using HMSRoomManagement.Common.Exceptions;
using HMSRoomManagement.Common.Service;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.DTO.RatePlan;
using HMSRoomManagement.Repository.Contracts;
using HMSRoomManagement.Service.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HMSRoomManagement.Service
{
    public class RatePlanService : IRatePlanService
    {
        private readonly IRatePlanRepository _ratePlanRepository;
        private readonly ICancellationPolicyRepository _policyRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly ICurrentUserService _currentUserService;

        public RatePlanService(
            IRatePlanRepository ratePlanRepository,
            ICancellationPolicyRepository policyRepository,
            IRoomTypeRepository roomTypeRepository,
            ICurrentUserService currentUserService)
        {
            _ratePlanRepository = ratePlanRepository;
            _policyRepository = policyRepository;
            _roomTypeRepository = roomTypeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<RatePlanDto>> GetRatePlansAsync(int? roomTypeId)
        {
            var ratePlans = await _ratePlanRepository.GetAllAsync();
            if (roomTypeId.HasValue)
                ratePlans = ratePlans.Where(rp => rp.RoomTypeId == roomTypeId.Value).ToList();

            if (!IsPlatformScope)
            {
                var ownTypeIds = (await _roomTypeRepository.GetAllAsync())
                    .Where(rt => rt.HotelId == _currentUserService.HotelId)
                    .Select(rt => rt.Id)
                    .ToHashSet();
                ratePlans = ratePlans.Where(rp => ownTypeIds.Contains(rp.RoomTypeId)).ToList();
            }

            var result = new List<RatePlanDto>();
            foreach (var rp in ratePlans)
                result.Add(await ComposeAsync(rp));

            return result;
        }

        public async Task<RatePlanDto> GetRatePlanByIdAsync(int id)
        {
            var rp = await _ratePlanRepository.GetAsync(id)
                ?? throw new NotFoundException($"Rate plan {id} not found.");

            await EnsureReadViaRoomTypeAsync(rp.RoomTypeId);
            return await ComposeAsync(rp);
        }

        public async Task<RatePlanDto> CreateRatePlanAsync(CreateRatePlanDto dto)
        {
            await EnsureHotelScopeViaRoomTypeAsync(dto.RoomTypeId);

            var ratePlan = new RatePlan
            {
                RoomTypeId = dto.RoomTypeId,
                PlanName = dto.PlanName,
                PricePerNight = dto.PricePerNight,
                Refundable = dto.Refundable,
                BreakfastIncluded = dto.BreakfastIncluded,
                PayAtHotel = dto.PayAtHotel,
                IsActive = dto.IsActive,
            };
            await _ratePlanRepository.AddAsync(ratePlan); // saves and assigns ratePlan.Id

            var policy = new CancellationPolicy
            {
                RatePlanId = ratePlan.Id,
                FreeCancellationUntilHours = dto.FreeCancellationUntilHours,
                PenaltyPercentage = dto.PenaltyPercentage,
            };
            await _policyRepository.AddAsync(policy);

            return await ComposeAsync(ratePlan);
        }

        public async Task UpdateRatePlanAsync(int id, UpdateRatePlanDto dto)
        {
            var ratePlan = await _ratePlanRepository.GetAsync(id)
                ?? throw new NotFoundException($"Rate plan {id} not found.");

            await EnsureHotelScopeViaRoomTypeAsync(ratePlan.RoomTypeId);

            ratePlan.PlanName = dto.PlanName;
            ratePlan.PricePerNight = dto.PricePerNight;
            ratePlan.Refundable = dto.Refundable;
            ratePlan.BreakfastIncluded = dto.BreakfastIncluded;
            ratePlan.PayAtHotel = dto.PayAtHotel;
            ratePlan.IsActive = dto.IsActive;
            await _ratePlanRepository.UpdateAsync(id, ratePlan);

            var existingPolicy = (await _policyRepository.Query().Where(p => p.RatePlanId == id).ToListAsync());
            var policy = existingPolicy.FirstOrDefault();
            if (policy != null)
            {
                policy.FreeCancellationUntilHours = dto.FreeCancellationUntilHours;
                policy.PenaltyPercentage = dto.PenaltyPercentage;
                await _policyRepository.UpdateAsync(policy.Id, policy);
            }
            else
            {
                await _policyRepository.AddAsync(new CancellationPolicy
                {
                    RatePlanId = id,
                    FreeCancellationUntilHours = dto.FreeCancellationUntilHours,
                    PenaltyPercentage = dto.PenaltyPercentage,
                });
            }
        }

        public async Task DeleteRatePlanAsync(int id)
        {
            var ratePlan = await _ratePlanRepository.GetAsync(id)
                ?? throw new NotFoundException($"Rate plan {id} not found.");

            await EnsureHotelScopeViaRoomTypeAsync(ratePlan.RoomTypeId);

            var policy = (await _policyRepository.Query().Where(p => p.RatePlanId == id).ToListAsync()).FirstOrDefault();
            if (policy != null)
                await _policyRepository.DeleteAsync(policy.Id);

            await _ratePlanRepository.DeleteAsync(id);
        }

        private async Task<RatePlanDto> ComposeAsync(RatePlan rp)
        {
            var policy = (await _policyRepository.Query().Where(p => p.RatePlanId == rp.Id).ToListAsync()).FirstOrDefault();

            return new RatePlanDto
            {
                Id = rp.Id,
                RoomTypeId = rp.RoomTypeId,
                PlanName = rp.PlanName,
                PricePerNight = rp.PricePerNight,
                Refundable = rp.Refundable,
                BreakfastIncluded = rp.BreakfastIncluded,
                PayAtHotel = rp.PayAtHotel,
                IsActive = rp.IsActive,
                FreeCancellationUntilHours = policy?.FreeCancellationUntilHours ?? 0,
                PenaltyPercentage = policy?.PenaltyPercentage ?? 0,
            };
        }

        private bool IsPlatformScope => _currentUserService.Role is Roles.SuperAdmin or Roles.TenantAdmin;

        private async Task EnsureReadViaRoomTypeAsync(int roomTypeId)
        {
            if (IsPlatformScope) return;
            var roomType = await _roomTypeRepository.GetAsync(roomTypeId)
                ?? throw new NotFoundException($"Room type {roomTypeId} not found.");
            if (roomType.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("You don't have access to this hotel.");
        }

        private async Task EnsureHotelScopeViaRoomTypeAsync(int roomTypeId)
        {
            var role = _currentUserService.Role;
            if (role is Roles.SuperAdmin or Roles.TenantAdmin) return;

            var roomType = await _roomTypeRepository.GetAsync(roomTypeId)
                ?? throw new NotFoundException($"Room type {roomTypeId} not found.");

            if (role is Roles.HotelOwner or Roles.HR && _currentUserService.HotelId == roomType.HotelId) return;

            throw new ForbiddenException("You don't have permission to manage rate plans for this hotel.");
        }
    }
}

using AuthService.Api.Constraints;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.DTO.RatePlanDTO;
using HMSRoomManagement.Repository.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoomManagementService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatePlansController : ControllerBase
    {
        private readonly IRatePlanRepository _ratePlanRepository;
        private readonly ICancellationPolicyRepository _policyRepository;

        public RatePlansController(IRatePlanRepository ratePlanRepository, ICancellationPolicyRepository policyRepository)
        {
            _ratePlanRepository = ratePlanRepository;
            _policyRepository = policyRepository;
        }

        private async Task<RatePlanDTO> ComposeAsync(RatePlan rp)
        {
            var policies = await _policyRepository.FindAsync(cp => cp.RatePlanId == rp.Id);
            var policy = policies.FirstOrDefault();

            return new RatePlanDTO
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RatePlanDTO>>> GetRatePlans([FromQuery] int? roomTypeId)
        {
            var ratePlans = roomTypeId.HasValue
                ? await _ratePlanRepository.FindAsync(rp => rp.RoomTypeId == roomTypeId.Value)
                : await _ratePlanRepository.GetAllAsync();

            var result = new List<RatePlanDTO>();
            foreach (var rp in ratePlans)
                result.Add(await ComposeAsync(rp));

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RatePlanDTO>> GetRatePlan(int id)
        {
            var rp = await _ratePlanRepository.GetByIdAsync(id);
            if (rp == null) return NotFound(new { message = $"Rate plan {id} not found." });
            return Ok(await ComposeAsync(rp));
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<ActionResult<RatePlanDTO>> CreateRatePlan([FromBody] CreateRatePlanDTO dto)
        {
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
            await _ratePlanRepository.AddAsync(ratePlan);
            await _ratePlanRepository.SaveChangesAsync(); // need ratePlan.Id before creating the policy row

            var policy = new CancellationPolicy
            {
                RatePlanId = ratePlan.Id,
                FreeCancellationUntilHours = dto.FreeCancellationUntilHours,
                PenaltyPercentage = dto.PenaltyPercentage,
            };
            await _policyRepository.AddAsync(policy);
            await _policyRepository.SaveChangesAsync();

            var result = await ComposeAsync(ratePlan);
            return CreatedAtAction(nameof(GetRatePlan), new { id = ratePlan.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.HotelOwner},{Roles.HR}")]
        public async Task<ActionResult<RatePlanDTO>> UpdateRatePlan(int id, [FromBody] UpdateRatePlanDTO dto)
        {
            var ratePlan = await _ratePlanRepository.GetByIdAsync(id);
            if (ratePlan == null) return NotFound(new { message = $"Rate plan {id} not found." });

            ratePlan.PlanName = dto.PlanName;
            ratePlan.PricePerNight = dto.PricePerNight;
            ratePlan.Refundable = dto.Refundable;
            ratePlan.BreakfastIncluded = dto.BreakfastIncluded;
            ratePlan.PayAtHotel = dto.PayAtHotel;
            ratePlan.IsActive = dto.IsActive;
            _ratePlanRepository.Update(ratePlan);
            await _ratePlanRepository.SaveChangesAsync();

            var existingPolicy = (await _policyRepository.FindAsync(cp => cp.RatePlanId == id)).FirstOrDefault();
            if (existingPolicy != null)
            {
                existingPolicy.FreeCancellationUntilHours = dto.FreeCancellationUntilHours;
                existingPolicy.PenaltyPercentage = dto.PenaltyPercentage;
                _policyRepository.Update(existingPolicy);
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
            await _policyRepository.SaveChangesAsync();

            return Ok(await ComposeAsync(ratePlan));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.HotelOwner}")]
        public async Task<IActionResult> DeleteRatePlan(int id)
        {
            var ratePlan = await _ratePlanRepository.GetByIdAsync(id);
            if (ratePlan == null) return NotFound(new { message = $"Rate plan {id} not found." });

            var policy = (await _policyRepository.FindAsync(cp => cp.RatePlanId == id)).FirstOrDefault();
            if (policy != null)
            {
                _policyRepository.Delete(policy);
                await _policyRepository.SaveChangesAsync();
            }

            _ratePlanRepository.Delete(ratePlan);
            await _ratePlanRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}

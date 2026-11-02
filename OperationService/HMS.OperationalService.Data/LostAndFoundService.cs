using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Common.Security;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.DTO;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using Status = HMS.OperationalService.Common.Enums.TaskStatus;

namespace HMS.OperationalService.Service
{
    public class LostAndFoundService : ILostAndFoundService
    {
        private readonly ILostAndFoundRepository _repository;
        private readonly IAccessGuard _guard;
        private readonly ICurrentUser _user;

        public LostAndFoundService(ILostAndFoundRepository repository, IAccessGuard guard, ICurrentUser user)
        {
            _repository = repository;
            _guard = guard;
            _user = user;
        }

        private void RequireAccess() => _guard.RequireAny(Module.FrontDesk, Module.Housekeeping);

        public async Task<IEnumerable<LostAndFound>> GetAllAsync()
        {
            RequireAccess();
            var hotelId = _guard.HotelId;
            var items = await _repository.FindAsync(i => i.HotelId == hotelId);
            return items.OrderByDescending(i => i.FoundDate).ToList();
        }

        public async Task<LostAndFound> GetByIdAsync(int id)
        {
            RequireAccess();
            return await GetOwnedAsync(id);
        }

        public async Task<LostAndFound> CreateAsync(LostAndFoundDto dto)
        {
            RequireAccess();

            var item = new LostAndFound
            {
                HotelId = _guard.HotelId,
                ItemCode = "LAF-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(),
                RoomId = dto.RoomId,
                ItemName = dto.ItemName,
                Description = dto.Description,
                FoundByStaffId = _user.StaffId ?? dto.FoundByStaffId,
                StorageLocation = dto.StorageLocation,
                FoundDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();
            return item;
        }

        public async Task ClaimItemAsync(int id, Guid guestId)
        {
            RequireAccess();
            var item = await GetOwnedAsync(id);
            if (item.IsClaimed) throw new BadRequestException("This item has already been claimed.");

            item.IsClaimed = true;
            item.ClaimedByGuestId = guestId;
            item.ClaimedDate = DateTime.UtcNow;

            _repository.Update(item);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            RequireAccess();
            _guard.RequireRole("HotelOwner", "HR", "Manager");
            _repository.Delete(await GetOwnedAsync(id));
            await _repository.SaveChangesAsync();
        }

        private async Task<LostAndFound> GetOwnedAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Lost and found record {id} not found.");
            if (item.HotelId != _guard.HotelId) throw new ForbiddenException("Access denied.");
            return item;
        }
    }
}

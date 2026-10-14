using HMS.OperationalService.Common.Exceptions;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Service
{
    public class LostAndFoundService : ILostAndFoundService
    {
        private readonly ILostAndFoundRepository _repository;

        public LostAndFoundService(ILostAndFoundRepository repository) => _repository = repository;

        public async Task<IEnumerable<LostAndFound>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<LostAndFound> GetByIdAsync(int id) =>
            await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"Lost and found record with ID {id} not found.");

        public async Task<LostAndFound> CreateAsync(LostAndFound item)
        {
            item.ItemCode = "LAF-" + Guid.NewGuid().ToString()[..6].ToUpper();
            item.FoundDate = DateTime.UtcNow;
            item.CreatedAt = DateTime.UtcNow;
            item.IsClaimed = false;
            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();
            return item;
        }

        public async Task ClaimItemAsync(int id, Guid guestId)
        {
            var item = await GetByIdAsync(id);
            item.IsClaimed = true;
            item.ClaimedByGuestId = guestId;
            item.ClaimedDate = DateTime.UtcNow;
            _repository.Update(item);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await GetByIdAsync(id);
            _repository.Delete(item);
            await _repository.SaveChangesAsync();
        }
    }
}

using HMS.GuestService.DTO.GuestDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.Service.Contracts
{
    public interface IGuestService
    {
        Task<IEnumerable<GuestDTO>> GetAllGuestsAsync();
        Task<GuestDTO> GetGuestByIdAsync(Guid id);
        Task<GuestDTO> CreateGuestAsync(CreateGuestDTO dto);
        Task UpdateGuestAsync(Guid id, UpdateGuestDTO dto);
        Task DeleteGuestAsync(Guid id);
    }
}

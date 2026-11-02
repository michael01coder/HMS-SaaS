using HMS.GuestService.DTO.ReservationDTO;
using Microsoft.EntityFrameworkCore;

namespace HMS.GuestService.Service.Contracts
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDTO>> GetAllAsync();
        Task<ReservationDTO> GetReservationByIdAsync(int reservationId);
        Task<ReservationDTO?> GetByCodeAsync(string reservationCode);
        Task<ReservationDTO> CreateReservationAsync(CreateReservationDTO dto);
        Task CancelReservationAsync(int reservationId);
    }
}
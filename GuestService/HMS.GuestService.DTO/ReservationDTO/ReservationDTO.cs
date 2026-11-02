namespace HMS.GuestService.DTO.ReservationDTO
{
    public class ReservationDTO : BaseReservationDTO
    {
        public int ReservationId { get; set; }
        public Guid HotelId { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

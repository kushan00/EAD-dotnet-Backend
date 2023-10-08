namespace backend.DTO;
    public class ReservationDTO
    {
    public String? Id { get; set; }
    public String? BookingId { get; set; } = null!;
    public String? User { get; set; }
    public String? Schedule { get; set; } = null!;
    public DateTime? BookedTime { get; set; } = null!;
    public DateTime? ReserveTime { get; set; } = null!;
    public String? StartCity { get; set; } = null!;
    public String? EndCity { get; set; } = null!;
    public int? PaxCount { get; set; } = null!;
    public int? Status { get; set; } = null!;
}

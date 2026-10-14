namespace Booking.API.Entities;

public class BookingRecord
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string UserId { get; set; } = default!;
    public int SeatCount { get; set; }
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
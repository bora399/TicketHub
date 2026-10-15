namespace Messaging.Contracts;

public record BookingCreatedEvent
{
    public Guid BookingId { get; init; }
    public Guid EventId { get; init; }
    public string UserId { get; init; } = default!;
    public int SeatCount { get; init; }
    public DateTime CreatedAt { get; init; }
}
using Catalog.API.Data;
using MassTransit;
using Messaging.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Consumers;

public class BookingCreatedConsumer : IConsumer<BookingCreatedEvent>
{
    private readonly CatalogDbContext _context;
    private readonly ILogger<BookingCreatedConsumer> _logger;

    public BookingCreatedConsumer(CatalogDbContext context, ILogger<BookingCreatedConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Yeni bilet eventi alındı. EventId: {EventId}, Adet: {SeatCount}", message.EventId, message.SeatCount);

        var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.Id == message.EventId);

        if (eventItem is null)
        {
            _logger.LogWarning("Etkinlik bulunamadı! EventId: {EventId}", message.EventId);
            return;
        }

        if (eventItem.AvailableSeats >= message.SeatCount)
        {
            eventItem.AvailableSeats -= message.SeatCount;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Koltuklar başarıyla düşürüldü. Kalan Koltuk: {RemainingSeats}", eventItem.AvailableSeats);
        }
        else
        {
            _logger.LogError("Yetersiz koltuk sayısı! EventId: {EventId}", message.EventId);
        }
    }
}
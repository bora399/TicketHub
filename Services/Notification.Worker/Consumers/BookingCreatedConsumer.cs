using MassTransit;
using Messaging.Contracts;
using Microsoft.Extensions.Logging;

namespace Notification.Worker.Consumers;

public class BookingCreatedConsumer : IConsumer<BookingCreatedEvent>
{
    private readonly ILogger<BookingCreatedConsumer> _logger;

    public BookingCreatedConsumer(ILogger<BookingCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("==================================================");
        _logger.LogInformation("📩 [BİLDİRİM SERVİSİ] Biletleme Eventi Yakalandı!");
        _logger.LogInformation("Kullanıcı: {UserId} | Bilet ID: {BookingId}", message.UserId, message.BookingId);
        _logger.LogInformation("Bilet Adedi: {SeatCount} | Satın Alma Zamanı: {CreatedAt}", message.SeatCount, message.CreatedAt);

        await Task.Delay(1000);

        _logger.LogInformation("✅ E-posta ve Bilet PDF'i başarıyla kullanıcıya iletildi.");
        _logger.LogInformation("==================================================");
    }
}
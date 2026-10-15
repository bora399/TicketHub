using Booking.API.Data;
using Booking.API.Entities;
using MassTransit;
using Messaging.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Booking.API.Controllers;

public record CreateBookingRequest(Guid EventId, string UserId, int SeatCount);

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public BookingsController(BookingDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _context.Bookings
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return Ok(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        var booking = new BookingRecord
        {
            Id = Guid.NewGuid(),
            EventId = request.EventId,
            UserId = request.UserId,
            SeatCount = request.SeatCount,
            Status = "Pending", 
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        await _publishEndpoint.Publish(new BookingCreatedEvent
        {
            BookingId = booking.Id,
            EventId = booking.EventId,
            UserId = booking.UserId,
            SeatCount = booking.SeatCount,
            CreatedAt = booking.CreatedAt
        });

        return Accepted(booking);//asenkron yaklasım oldugundan ok yerine accepted dondururuz.
    }
}
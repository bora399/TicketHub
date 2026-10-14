using Booking.API.Data;
using Booking.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Booking.API.Controllers;

public record CreateBookingRequest(Guid EventId, string UserId, int SeatCount);

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BookingDbContext _context;

    public BookingsController(IHttpClientFactory httpClientFactory, BookingDbContext context)
    {
        _httpClientFactory = httpClientFactory;
        _context = context;
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
        var client = _httpClientFactory.CreateClient("CatalogService");

        var response = await client.PostAsJsonAsync($"api/events/{request.EventId}/reserve-seats", request.SeatCount);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(new { Message = "Katalog servisinden onay alınamadı", Detail = error });
        }

        var booking = new BookingRecord
        {
            Id = Guid.NewGuid(),
            EventId = request.EventId,
            UserId = request.UserId,
            SeatCount = request.SeatCount,
            Status = "Confirmed",
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return Ok(booking);
    }
}
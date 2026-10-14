using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers;

public record CreateBookingRequest(Guid EventId, string UserId, int SeatCount);//dtoları c# 9 ile gelen record ile oluşturuyoruz.

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BookingsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
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

        var bookingId = Guid.NewGuid();

        return Ok(new
        {
            BookingId = bookingId,
            Status = "Confirmed",
            EventId = request.EventId,
            Seats = request.SeatCount
        });
    }
}
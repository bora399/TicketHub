using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

public record EventDto(Guid Id, string Name, DateTime Date, decimal Price, int AvailableSeats);

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private static readonly List<EventDto> _events = new()
    {
        new EventDto(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Mor ve Ötesi Konseri", DateTime.UtcNow.AddDays(10), 750m, 100),
        new EventDto(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Şampiyonlar Ligi Final Gösterimi", DateTime.UtcNow.AddDays(25), 300m, 5)
    };

    [HttpGet]
    public IActionResult GetAll() => Ok(_events);

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var ev = _events.FirstOrDefault(e => e.Id == id);
        if (ev is null) return NotFound(new { Message = "Etkinlik bulunamadı." });

        return Ok(ev);
    }

    [HttpPost("{id:guid}/reserve-seats")]
    public IActionResult ReserveSeats(Guid id, [FromBody] int seatCount)
    {
        var index = _events.FindIndex(e => e.Id == id);
        if (index == -1) return NotFound("Etkinlik bulunamadı.");

        var ev = _events[index];
        if (ev.AvailableSeats < seatCount)
            return BadRequest("Yeterli boş koltuk yok!");

        _events[index] = ev with { AvailableSeats = ev.AvailableSeats - seatCount };

        return Ok(new { Message = $"{seatCount} koltuk ayrıldı.", RemainingSeats = _events[index].AvailableSeats });
    }
}
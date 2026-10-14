using Catalog.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly CatalogDbContext _context;
    public EventsController(CatalogDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _context.Events.AsNoTracking().ToListAsync();
        return Ok(events);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ev = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        if (ev is null) return NotFound(new { Message = "Etkinlik bulunamadı." });

        return Ok(ev);
    }

    [HttpPost("{id:guid}/reserve-seats")]
    public async Task<IActionResult> ReserveSeats(Guid id, [FromBody] int seatCount)
    {
        var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
        if (ev is null) return NotFound("Etkinlik bulunamadı.");

        if (ev.AvailableSeats < seatCount)
            return BadRequest("Yeterli boş koltuk yok!");

        ev.AvailableSeats -= seatCount;
        await _context.SaveChangesAsync();

        return Ok(new { Message = $"{seatCount} koltuk ayrıldı.", RemainingSeats = ev.AvailableSeats });
    }
}
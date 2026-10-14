namespace Catalog.API.Entities;

public class EventItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
}
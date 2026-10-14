using Booking.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.API.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<BookingRecord> Bookings => Set<BookingRecord>();
}
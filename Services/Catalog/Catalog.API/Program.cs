using Catalog.API.Data;
using Catalog.API.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Events.Any())
    {
        dbContext.Events.AddRange(//seed for test
            new EventItem
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Mor ve Ötesi Konseri",
                Date = DateTime.UtcNow.AddDays(10),
                Price = 750m,
                AvailableSeats = 100
            },
            new EventItem
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Þampiyonlar Ligi Final Gösterimi",
                Date = DateTime.UtcNow.AddDays(25),
                Price = 300m,
                AvailableSeats = 5
            }
        );
        dbContext.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

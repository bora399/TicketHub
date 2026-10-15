using MassTransit;
using Notification.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("notification", false));

    x.AddConsumer<BookingCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("127.0.0.1", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();
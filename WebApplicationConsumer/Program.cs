using Message;
using WebApplicationConsumer.Consumer;
using WebApplicationConsumer.Service;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("Rabbitmq"));

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddHostedService<RabbitConsumer>();



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

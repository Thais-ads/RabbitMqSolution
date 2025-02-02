using Message;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("Rabbitmq"));


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

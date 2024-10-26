using Message;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using WebApplicationConsumer.Service;

namespace WebApplicationConsumer.Consumer
{
    public class RabbitConsumer : BackgroundService
    {

        private readonly RabbitMqConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;


        public RabbitConsumer(IOptions<RabbitMqConfiguration> options, IServiceProvider service)
        {
            _config = options.Value;
            _serviceProvider = service;


            var factory = new ConnectionFactory
            {
                HostName = _config.Host
            };


            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                        queue: _config.Queue,
                        durable: false,
                        autoDelete: false,
                        exclusive: false,
            arguments: null);

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, eventArgs) =>
            {

                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);
                var message = JsonConvert.DeserializeObject<MessageRabbit>(contentString);

                NotfyUser(message);


                _channel.BasicAck(eventArgs.DeliveryTag,false);

            };

            _channel.BasicConsume(_config.Queue,false,consumer);


            return Task.CompletedTask;
        }


        public void NotfyUser(MessageRabbit message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                notificationsService.NotfyUser(message.FromId,message.ToId,message.Content);
            }

        }

    }
}

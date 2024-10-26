using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using Message;
using Microsoft.Extensions.Options;


namespace WebApplicationPublish.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ConnectionFactory _factory;
        private readonly RabbitMqConfiguration _config;

        public MessageController(IOptions<RabbitMqConfiguration> options)
        {
            _config = options.Value;

            _factory = new ConnectionFactory
            {

                HostName = _config.Host,

            };
        }


        [HttpPost]
        public IActionResult PostMessage([FromBody] MessageRabbit message)
        {
            using (var connection = _factory.CreateConnection())
            {

                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: _config.Queue,
                        durable: false,
                        autoDelete: false,
                        exclusive: false,
                        arguments: null
                        );


                    var messageSerialized = JsonConvert.SerializeObject(message);
                    var messagebytes = Encoding.UTF8.GetBytes(messageSerialized);


                    channel.BasicPublish(
                        exchange:"",
                        routingKey: _config.Queue,
                        basicProperties:null,
                        body: messagebytes
                     );


                }


                return Accepted();


            }
        }
    }
}

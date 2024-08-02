using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System.Text;

namespace ThirdPartyUtilities.Implementation
{
    public class QueueServiceProducer
    {
        public string _hostname;
        public QueueServiceProducer(string hostName) {
        _hostname = hostName;
        }
        public void Produce(string queueName, JObject body)
        {
            ConnectionFactory _connectionFactory = new ConnectionFactory { HostName = _hostname };
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            var message = JsonConvert.SerializeObject(body);
            var encodeMessage = Encoding.UTF8.GetBytes(message);
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: encodeMessage);
            Console.WriteLine($"Message Published {message}");
        }
    }
}

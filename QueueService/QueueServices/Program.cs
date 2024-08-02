using System.Text;
using Newtonsoft.Json.Linq;
using QueueServices.ConsumerClasses;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using QueueServices.Configuration;
using QueueServices.Models.Queue;
using QueueServices.Helpers;

internal class Program
{
    private static void Main(string[] args)
    {
        string enviourment = AppConfiguration.GetConfiguration("Queue:enviourment");
        string hostName = AppConfiguration.GetConfiguration("QueueServers:hotName");
        var factory = new ConnectionFactory { HostName = hostName };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var serviceProvider = new ServiceCollection()
        .AddSingleton<ApplicationDBContext>()
        .BuildServiceProvider();
        var appDB= serviceProvider.GetService<ApplicationDBContext>();
        channel.QueueDeclare(queue: enviourment + "pushNotificationQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        var pushNotificationQueue = new EventingBasicConsumer(channel);
        pushNotificationQueue.Received += async (model, e) =>
        {
            var body = e.Body.ToArray();
            var encoded = Encoding.UTF8.GetString(body);
            JObject message = new JObject();
            message = JObject.Parse(encoded);
            var consumer = new PushNotificationQueueConsumer();
            CancellationToken ctoken = new CancellationToken();
            Admin_GetPushNotificationByIdResponseModel res = await consumer.PushNotificationConsume(message, ctoken,appDB);
            Console.WriteLine(res);
        };
        channel.BasicConsume(queue: enviourment+"pushNotificationQueue", autoAck: true, consumer: pushNotificationQueue);

        Console.ReadKey();
    }
}
using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQConsumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configuration
            var hostName = "localhost";
            var queueName = "my_queue";
            var userName = "guest";
            var password = "guest";

            Console.WriteLine("RabbitMQ Consumer Starting...");

            try
            {
                // Create connection factory
                var factory = new ConnectionFactory
                {
                    HostName = hostName,
                    UserName = userName,
                    Password = password
                };

                // Create connection and channel (async for RabbitMQ.Client 7.0+)
                await using var connection = await factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                // Declare the queue (ensures it exists)
                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                Console.WriteLine($"Connected to RabbitMQ. Waiting for messages on queue '{queueName}'...");
                Console.WriteLine("Press [Ctrl+C] to exit");

                // Create consumer
                var consumer = new AsyncEventingBasicConsumer(channel);
                
                // Handle received messages
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    
                    Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] Received message:");
                    Console.WriteLine($"  Content: {message}");
                    Console.WriteLine($"  Routing Key: {routingKey}");
                    
                    // Acknowledge the message
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    Console.WriteLine("  Message acknowledged");
                };

                // Start consuming
                await channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: false,  // Manual acknowledgment
                    consumer: consumer
                );

                // Keep the application running
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}

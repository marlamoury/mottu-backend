using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MottuBackend.Application.Interfaces;
using MottuBackend.Domain.Entities;
using MottuBackend.Infrastructure.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MottuBackend.Infrastructure.Services;

public class MessageService : IMessageService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly MongoDbContext _context;
    private readonly ILogger<MessageService> _logger;

    public MessageService(MongoDbContext context, ILogger<MessageService> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;

        var host = configuration["RabbitMQ:HostName"] ?? "localhost";
        var port = int.TryParse(configuration["RabbitMQ:Port"], out var p) ? p : 5672;
        var user = configuration["RabbitMQ:UserName"] ?? "guest";
        var pass = configuration["RabbitMQ:Password"] ?? "guest";
        var vhost = configuration["RabbitMQ:VHost"] ?? "/";

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = user,
            Password = pass,
            VirtualHost = vhost,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "motorcycle.events", type: ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(queue: "motorcycle.registered", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queue: "motorcycle.registered", exchange: "motorcycle.events", routingKey: "motorcycle.registered");

        _logger.LogInformation("MessageService connected to RabbitMQ at {Host}:{Port}", host, port);
    }

    public async Task PublishMotorcycleRegisteredAsync(Guid motorcycleId, string identifier, int year, string model, string licensePlate)
    {
        try
        {
            var message = new
            {
                MotorcycleId = motorcycleId,
                Identifier = identifier,
                Year = year,
                Model = model,
                LicensePlate = licensePlate,
                Timestamp = DateTime.UtcNow
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish(
                exchange: "motorcycle.events",
                routingKey: "motorcycle.registered",
                basicProperties: null,
                body: body);

            _logger.LogInformation("Published motorcycle.registered for {MotorcycleId}", motorcycleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing motorcycle.registered");
            throw;
        }
    }

    public async Task ConsumeMotorcycleNotificationsAsync()
    {
        try
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Message received: {Message}", message);

                try
                {
                    var motorcycleEvent = JsonSerializer.Deserialize<MotorcycleRegisteredEvent>(message);
                    if (motorcycleEvent != null && motorcycleEvent.Year == 2024)
                    {
                        var notification = new MotorcycleNotification
                        {
                            Id = Guid.NewGuid(),
                            MotorcycleId = motorcycleEvent.MotorcycleId,
                            Message = $"Motorcycle {motorcycleEvent.Identifier} from year 2024 was registered",
                            CreatedAt = DateTime.UtcNow
                        };

                        await _context.MotorcycleNotifications.InsertOneAsync(notification);
                        _logger.LogInformation("Stored notification for 2024 motorcycle {MotorcycleId}", motorcycleEvent.MotorcycleId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing motorcycle notification");
                }
            };

            _channel.BasicConsume(
                queue: "motorcycle.registered",
                autoAck: true,
                consumer: consumer);

            _logger.LogInformation("Started consuming motorcycle notifications");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up message consumer");
            throw;
        }
    }

    public void Dispose()
    {
        try { _channel?.Close(); } catch { }
        try { _connection?.Close(); } catch { }
        _channel?.Dispose();
        _connection?.Dispose();
    }

    private class MotorcycleRegisteredEvent
    {
        public Guid MotorcycleId { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Model { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
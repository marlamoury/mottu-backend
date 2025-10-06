using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MottuBackend.Application.Interfaces;

namespace MottuBackend.API.Background;

public class MessageConsumerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageConsumerHostedService> _logger;

    public MessageConsumerHostedService(IServiceProvider serviceProvider, ILogger<MessageConsumerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting RabbitMQ consumer...");
        
        using var scope = _serviceProvider.CreateScope();
        var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
        
        await messageService.ConsumeMotorcycleNotificationsAsync();
    }
}
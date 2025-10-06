using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MottuBackend.Application.Interfaces;
using MottuBackend.Infrastructure.Data;
using MottuBackend.Infrastructure.Repositories;
using MottuBackend.Infrastructure.Services;

namespace MottuBackend.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB
        var mongoConnectionString = configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
        var mongoDatabaseName = configuration["MongoDB:DatabaseName"] ?? "mottu_backend";

        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
        services.AddScoped<IMongoDatabase>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoDatabaseName);
        });
        services.AddScoped<MongoDbContext>();

        // Repositories
        services.AddScoped<IMotorcycleRepository, MongoMotorcycleRepository>();
        services.AddScoped<IDeliveryDriverRepository, MongoDeliveryDriverRepository>();
        services.AddScoped<IRentalRepository, MongoRentalRepository>();

        // Services
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IMessageService, MessageService>();

        return services;
    }
}
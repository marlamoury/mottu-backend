using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using MottuBackend.Application.Interfaces;
using MottuBackend.Application.Services;

namespace MottuBackend.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IMotorcycleService, MotorcycleService>();
        services.AddScoped<IDeliveryDriverService, DeliveryDriverService>();
        services.AddScoped<IRentalService, RentalService>();
        
        // AutoMapper
        services.AddAutoMapper(typeof(MottuBackend.API.Mapping.MappingProfile));
        
        return services;
    }
}

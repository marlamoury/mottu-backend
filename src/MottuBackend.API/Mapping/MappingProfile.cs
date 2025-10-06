using AutoMapper;
using MottuBackend.Application.DTOs;
using MottuBackend.Domain.Entities;

namespace MottuBackend.API.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Motorcycle mappings
        CreateMap<Motorcycle, MotorcycleDto>();
        CreateMap<CreateMotorcycleDto, Motorcycle>();
        CreateMap<UpdateMotorcycleDto, Motorcycle>();

        // DeliveryDriver mappings
        CreateMap<DeliveryDriver, DeliveryDriverDto>();
        CreateMap<CreateDeliveryDriverDto, DeliveryDriver>();

        // Rental mappings
        CreateMap<Rental, RentalDto>();
        CreateMap<CreateRentalDto, Rental>();
    }
}

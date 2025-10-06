using MottuBackend.Application.DTOs;

namespace MottuBackend.Application.Interfaces;

public interface IDeliveryDriverService
{
    Task<DeliveryDriverDto> CreateAsync(CreateDeliveryDriverDto createDto);
    Task<IEnumerable<DeliveryDriverDto>> GetAllAsync();
    Task<DeliveryDriverDto?> GetByIdAsync(Guid id);
    Task<DeliveryDriverDto> UpdateLicenseImageAsync(Guid id, UpdateLicenseImageDto updateDto);
}

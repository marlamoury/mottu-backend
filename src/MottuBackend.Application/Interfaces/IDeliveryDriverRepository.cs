using MottuBackend.Domain.Entities;

namespace MottuBackend.Application.Interfaces;

public interface IDeliveryDriverRepository
{
    Task<DeliveryDriver?> GetByIdAsync(Guid id);
    Task<DeliveryDriver?> GetByCnpjAsync(string cnpj);
    Task<DeliveryDriver?> GetByLicenseNumberAsync(string licenseNumber);
    Task<IEnumerable<DeliveryDriver>> GetAllAsync();
    Task<DeliveryDriver> CreateAsync(DeliveryDriver deliveryDriver);
    Task<DeliveryDriver> UpdateAsync(DeliveryDriver deliveryDriver);
}

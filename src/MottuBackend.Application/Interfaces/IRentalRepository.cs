using MottuBackend.Domain.Entities;

namespace MottuBackend.Application.Interfaces;

public interface IRentalRepository
{
    Task<Rental?> GetByIdAsync(Guid id);
    Task<IEnumerable<Rental>> GetAllAsync();
    Task<IEnumerable<Rental>> GetByDeliveryDriverIdAsync(Guid deliveryDriverId);
    Task<Rental> CreateAsync(Rental rental);
    Task<Rental> UpdateAsync(Rental rental);
}

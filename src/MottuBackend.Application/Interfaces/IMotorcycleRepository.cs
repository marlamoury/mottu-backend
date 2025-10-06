using MottuBackend.Domain.Entities;

namespace MottuBackend.Application.Interfaces;

public interface IMotorcycleRepository
{
    Task<Motorcycle?> GetByIdAsync(Guid id);
    Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<Motorcycle>> GetAllAsync(string? licensePlate = null);
    Task<Motorcycle> CreateAsync(Motorcycle motorcycle);
    Task<Motorcycle> UpdateAsync(Motorcycle motorcycle);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> HasActiveRentalsAsync(Guid motorcycleId);
}

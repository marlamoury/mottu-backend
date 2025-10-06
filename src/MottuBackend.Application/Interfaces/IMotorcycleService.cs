using MottuBackend.Application.DTOs;

namespace MottuBackend.Application.Interfaces;

public interface IMotorcycleService
{
    Task<MotorcycleDto> CreateAsync(CreateMotorcycleDto createDto);
    Task<IEnumerable<MotorcycleDto>> GetAllAsync(string? licensePlate = null);
    Task<MotorcycleDto?> GetByIdAsync(Guid id);
    Task<MotorcycleDto> UpdateAsync(Guid id, UpdateMotorcycleDto updateDto);
    Task<bool> DeleteAsync(Guid id);
}

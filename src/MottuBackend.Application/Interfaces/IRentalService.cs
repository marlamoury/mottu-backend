using MottuBackend.Application.DTOs;

namespace MottuBackend.Application.Interfaces;

public interface IRentalService
{
    Task<RentalDto> CreateAsync(CreateRentalDto createDto);
    Task<IEnumerable<RentalDto>> GetAllAsync();
    Task<RentalDto?> GetByIdAsync(Guid id);
    Task<RentalCalculationDto> CalculateReturnAsync(Guid id, ReturnRentalDto returnDto);
    Task<RentalDto> ReturnAsync(Guid id, ReturnRentalDto returnDto);
}

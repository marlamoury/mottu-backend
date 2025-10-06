using AutoMapper;
using Microsoft.Extensions.Logging;
using MottuBackend.Application.DTOs;
using MottuBackend.Application.Interfaces;
using MottuBackend.Domain.Entities;

namespace MottuBackend.Application.Services;

public class MotorcycleService : IMotorcycleService
{
    private readonly IMotorcycleRepository _repository;
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;
    private readonly ILogger<MotorcycleService> _logger;

    public MotorcycleService(
        IMotorcycleRepository repository,
        IMessageService messageService,
        IMapper mapper,
        ILogger<MotorcycleService> logger)
    {
        _repository = repository;
        _messageService = messageService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MotorcycleDto> CreateAsync(CreateMotorcycleDto createDto)
    {
        _logger.LogInformation("Creating motorcycle with license plate: {LicensePlate}", createDto.LicensePlate);

        // Check if license plate already exists
        var existingMotorcycle = await _repository.GetByLicensePlateAsync(createDto.LicensePlate);
        if (existingMotorcycle != null)
        {
            throw new InvalidOperationException($"Motorcycle with license plate '{createDto.LicensePlate}' already exists.");
        }

        var motorcycle = _mapper.Map<Motorcycle>(createDto);
        motorcycle.Id = Guid.NewGuid();
        
        var createdMotorcycle = await _repository.CreateAsync(motorcycle);
        
        // Publish motorcycle registered event
        await _messageService.PublishMotorcycleRegisteredAsync(
            createdMotorcycle.Id,
            createdMotorcycle.Identifier,
            createdMotorcycle.Year,
            createdMotorcycle.Model,
            createdMotorcycle.LicensePlate);

        _logger.LogInformation("Motorcycle created successfully with ID: {Id}", createdMotorcycle.Id);
        return _mapper.Map<MotorcycleDto>(createdMotorcycle);
    }

    public async Task<IEnumerable<MotorcycleDto>> GetAllAsync(string? licensePlate = null)
    {
        _logger.LogDebug("Getting all motorcycles with license plate filter: {LicensePlate}", licensePlate);
        
        var motorcycles = await _repository.GetAllAsync(licensePlate);
        return _mapper.Map<IEnumerable<MotorcycleDto>>(motorcycles);
    }

    public async Task<MotorcycleDto?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Getting motorcycle by ID: {Id}", id);
        
        var motorcycle = await _repository.GetByIdAsync(id);
        return motorcycle != null ? _mapper.Map<MotorcycleDto>(motorcycle) : null;
    }

    public async Task<MotorcycleDto> UpdateAsync(Guid id, UpdateMotorcycleDto updateDto)
    {
        _logger.LogInformation("Updating motorcycle with ID: {Id}", id);

        var motorcycle = await _repository.GetByIdAsync(id);
        if (motorcycle == null)
        {
            throw new KeyNotFoundException($"Motorcycle with ID '{id}' not found.");
        }

        // Check if new license plate already exists
        if (motorcycle.LicensePlate != updateDto.LicensePlate)
        {
            var existingMotorcycle = await _repository.GetByLicensePlateAsync(updateDto.LicensePlate);
            if (existingMotorcycle != null)
            {
                throw new InvalidOperationException($"Motorcycle with license plate '{updateDto.LicensePlate}' already exists.");
            }
        }

        motorcycle.LicensePlate = updateDto.LicensePlate;
        var updatedMotorcycle = await _repository.UpdateAsync(motorcycle);
        
        _logger.LogInformation("Motorcycle updated successfully with ID: {Id}", updatedMotorcycle.Id);
        return _mapper.Map<MotorcycleDto>(updatedMotorcycle);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting motorcycle with ID: {Id}", id);

        var motorcycle = await _repository.GetByIdAsync(id);
        if (motorcycle == null)
        {
            return false;
        }

        // Check if motorcycle has active rentals
        var hasActiveRentals = await _repository.HasActiveRentalsAsync(id);
        if (hasActiveRentals)
        {
            throw new InvalidOperationException("Cannot delete motorcycle with active rentals.");
        }

        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            _logger.LogInformation("Motorcycle deleted successfully with ID: {Id}", id);
        }
        return result;
    }
}

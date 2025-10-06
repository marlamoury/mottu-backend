using AutoMapper;
using Microsoft.Extensions.Logging;
using MottuBackend.Application.DTOs;
using MottuBackend.Application.Interfaces;
using MottuBackend.Domain.Entities;

namespace MottuBackend.Application.Services;

public class DeliveryDriverService : IDeliveryDriverService
{
    private readonly IDeliveryDriverRepository _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<DeliveryDriverService> _logger;

    public DeliveryDriverService(
        IDeliveryDriverRepository repository,
        IFileStorageService fileStorageService,
        IMapper mapper,
        ILogger<DeliveryDriverService> logger)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DeliveryDriverDto> CreateAsync(CreateDeliveryDriverDto createDto)
    {
        _logger.LogInformation("Creating delivery driver with identifier: {Identifier}", createDto.Identifier);

        // Validate license type
        if (!IsValidLicenseType(createDto.LicenseType))
        {
            throw new ArgumentException("Invalid license type. Must be A, B or A+B.");
        }

        // Check if CNPJ already exists
        var existingByCnpj = await _repository.GetByCnpjAsync(createDto.Cnpj);
        if (existingByCnpj != null)
        {
            throw new InvalidOperationException($"Delivery driver with CNPJ '{createDto.Cnpj}' already exists.");
        }

        // Check if license number already exists
        var existingByLicense = await _repository.GetByLicenseNumberAsync(createDto.LicenseNumber);
        if (existingByLicense != null)
        {
            throw new InvalidOperationException($"Delivery driver with license number '{createDto.LicenseNumber}' already exists.");
        }

        var deliveryDriver = _mapper.Map<DeliveryDriver>(createDto);
        deliveryDriver.Id = Guid.NewGuid();
        
        var createdDeliveryDriver = await _repository.CreateAsync(deliveryDriver);
        
        _logger.LogInformation("Delivery driver created successfully with ID: {Id}", createdDeliveryDriver.Id);
        return _mapper.Map<DeliveryDriverDto>(createdDeliveryDriver);
    }

    public async Task<IEnumerable<DeliveryDriverDto>> GetAllAsync()
    {
        _logger.LogDebug("Getting all delivery drivers");
        
        var deliveryDrivers = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<DeliveryDriverDto>>(deliveryDrivers);
    }

    public async Task<DeliveryDriverDto?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Getting delivery driver by ID: {Id}", id);
        
        var deliveryDriver = await _repository.GetByIdAsync(id);
        return deliveryDriver != null ? _mapper.Map<DeliveryDriverDto>(deliveryDriver) : null;
    }

    public async Task<DeliveryDriverDto> UpdateLicenseImageAsync(Guid id, UpdateLicenseImageDto updateDto)
    {
        _logger.LogInformation("Updating license image for delivery driver with ID: {Id}", id);

        var deliveryDriver = await _repository.GetByIdAsync(id);
        if (deliveryDriver == null)
        {
            throw new KeyNotFoundException($"Delivery driver with ID '{id}' not found.");
        }

        // Validate file format
        var allowedExtensions = new[] { ".png", ".bmp" };
        var fileExtension = Path.GetExtension(updateDto.LicenseImage.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new ArgumentException("Invalid file format. Only PNG and BMP files are allowed.");
        }

        // Save file
        var fileName = $"license_{id}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
        var filePath = await _fileStorageService.SaveFileAsync(updateDto.LicenseImage, "licenses");
        
        // Update delivery driver
        deliveryDriver.LicenseImageUrl = filePath;
        var updatedDeliveryDriver = await _repository.UpdateAsync(deliveryDriver);
        
        _logger.LogInformation("License image updated successfully for delivery driver with ID: {Id}", updatedDeliveryDriver.Id);
        return _mapper.Map<DeliveryDriverDto>(updatedDeliveryDriver);
    }

    private static bool IsValidLicenseType(string licenseType)
    {
        var validTypes = new[] { "A", "B", "A+B" };
        return validTypes.Contains(licenseType);
    }
}

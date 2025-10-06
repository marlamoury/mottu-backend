using AutoMapper;
using Microsoft.Extensions.Logging;
using MottuBackend.Application.DTOs;
using MottuBackend.Application.Interfaces;
using MottuBackend.Domain.Entities;

namespace MottuBackend.Application.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IDeliveryDriverRepository _deliveryDriverRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RentalService> _logger;

    private readonly Dictionary<int, decimal> _dailyRates = new()
    {
        { 7, 30.00m },
        { 15, 28.00m },
        { 30, 22.00m },
        { 45, 20.00m },
        { 50, 18.00m }
    };

    public RentalService(
        IRentalRepository rentalRepository,
        IMotorcycleRepository motorcycleRepository,
        IDeliveryDriverRepository deliveryDriverRepository,
        IMapper mapper,
        ILogger<RentalService> logger)
    {
        _rentalRepository = rentalRepository;
        _motorcycleRepository = motorcycleRepository;
        _deliveryDriverRepository = deliveryDriverRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<RentalDto> CreateAsync(CreateRentalDto createDto)
    {
        _logger.LogInformation("Creating rental for motorcycle {MotorcycleId} and driver {DeliveryDriverId}", 
            createDto.MotorcycleId, createDto.DeliveryDriverId);

        // Validate plan days
        if (!_dailyRates.ContainsKey(createDto.PlanDays))
        {
            throw new ArgumentException("Invalid plan days. Available plans: 7, 15, 30, 45, 50 days.");
        }

        // Check if motorcycle exists
        var motorcycle = await _motorcycleRepository.GetByIdAsync(createDto.MotorcycleId);
        if (motorcycle == null)
        {
            throw new KeyNotFoundException($"Motorcycle with ID '{createDto.MotorcycleId}' not found.");
        }

        // Check if delivery driver exists and has valid license
        var deliveryDriver = await _deliveryDriverRepository.GetByIdAsync(createDto.DeliveryDriverId);
        if (deliveryDriver == null)
        {
            throw new KeyNotFoundException($"Delivery driver with ID '{createDto.DeliveryDriverId}' not found.");
        }

        if (!deliveryDriver.LicenseType.Contains("A"))
        {
            throw new InvalidOperationException("Only delivery drivers with license type A can rent motorcycles.");
        }

        // Check if delivery driver already has active rental
        var existingRentals = await _rentalRepository.GetByDeliveryDriverIdAsync(createDto.DeliveryDriverId);
        var hasActiveRental = existingRentals.Any(r => r.EndDate > DateTime.UtcNow);
        if (hasActiveRental)
        {
            throw new InvalidOperationException("Delivery driver already has an active rental.");
        }

        var startDate = DateTime.UtcNow.Date.AddDays(1); // Starts tomorrow
        var expectedEndDate = startDate.AddDays(createDto.PlanDays);
        var dailyRate = _dailyRates[createDto.PlanDays];
        var totalAmount = createDto.PlanDays * dailyRate;

        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            MotorcycleId = createDto.MotorcycleId,
            DeliveryDriverId = createDto.DeliveryDriverId,
            StartDate = startDate,
            EndDate = expectedEndDate, // Will be updated on return
            ExpectedEndDate = expectedEndDate,
            PlanDays = createDto.PlanDays,
            DailyRate = dailyRate,
            TotalAmount = totalAmount
        };

        var createdRental = await _rentalRepository.CreateAsync(rental);
        
        _logger.LogInformation("Rental created successfully with ID: {Id}", createdRental.Id);
        return _mapper.Map<RentalDto>(createdRental);
    }

    public async Task<IEnumerable<RentalDto>> GetAllAsync()
    {
        _logger.LogDebug("Getting all rentals");
        
        var rentals = await _rentalRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RentalDto>>(rentals);
    }

    public async Task<RentalDto?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Getting rental by ID: {Id}", id);
        
        var rental = await _rentalRepository.GetByIdAsync(id);
        return rental != null ? _mapper.Map<RentalDto>(rental) : null;
    }

    public async Task<RentalCalculationDto> CalculateReturnAsync(Guid id, ReturnRentalDto returnDto)
    {
        _logger.LogInformation("Calculating return for rental with ID: {Id}", id);

        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null)
        {
            throw new KeyNotFoundException($"Rental with ID '{id}' not found.");
        }

        var returnDate = returnDto.ReturnDate.Date;
        var expectedEndDate = rental.ExpectedEndDate.Date;
        var dayDifference = (returnDate - expectedEndDate).Days;

        decimal fineAmount = 0;
        decimal additionalAmount = 0;
        string calculationDetails;

        if (dayDifference < 0) // Early return
        {
            var unusedDays = Math.Abs(dayDifference);
            var unusedAmount = unusedDays * rental.DailyRate;
            
            fineAmount = rental.PlanDays switch
            {
                7 => unusedAmount * 0.20m, // 20% fine
                15 => unusedAmount * 0.40m, // 40% fine
                _ => 0 // No fine for other plans
            };
            
            calculationDetails = $"Early return by {unusedDays} days. Fine: {fineAmount:C}";
        }
        else if (dayDifference > 0) // Late return
        {
            additionalAmount = dayDifference * 50.00m; // R$50 per additional day
            calculationDetails = $"Late return by {dayDifference} days. Additional cost: {additionalAmount:C}";
        }
        else
        {
            calculationDetails = "Return on expected date. No additional costs.";
        }

        return new RentalCalculationDto
        {
            TotalAmount = rental.TotalAmount + fineAmount + additionalAmount,
            FineAmount = fineAmount > 0 ? fineAmount : null,
            AdditionalAmount = additionalAmount > 0 ? additionalAmount : null,
            CalculationDetails = calculationDetails
        };
    }

    public async Task<RentalDto> ReturnAsync(Guid id, ReturnRentalDto returnDto)
    {
        _logger.LogInformation("Processing return for rental with ID: {Id}", id);

        var rental = await _rentalRepository.GetByIdAsync(id);
        if (rental == null)
        {
            throw new KeyNotFoundException($"Rental with ID '{id}' not found.");
        }

        var calculation = await CalculateReturnAsync(id, returnDto);
        
        rental.EndDate = returnDto.ReturnDate;
        rental.FineAmount = calculation.FineAmount;
        rental.AdditionalAmount = calculation.AdditionalAmount;
        rental.TotalAmount = calculation.TotalAmount;
        rental.UpdatedAt = DateTime.UtcNow;

        var updatedRental = await _rentalRepository.UpdateAsync(rental);
        
        _logger.LogInformation("Rental returned successfully with ID: {Id}. Total amount: {TotalAmount:C}", 
            updatedRental.Id, updatedRental.TotalAmount);
        return _mapper.Map<RentalDto>(updatedRental);
    }
}

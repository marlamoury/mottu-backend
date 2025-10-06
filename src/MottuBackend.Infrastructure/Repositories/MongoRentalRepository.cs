using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using MottuBackend.Application.Interfaces;
using MottuBackend.Domain.Entities;
using MottuBackend.Infrastructure.Data;

namespace MottuBackend.Infrastructure.Repositories;

public class MongoRentalRepository : IRentalRepository
{
    private readonly MongoDbContext _context;
    private readonly ILogger<MongoRentalRepository> _logger;

    public MongoRentalRepository(MongoDbContext context, ILogger<MongoRentalRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Rental?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Getting rental by ID: {Id}", id);
        var filter = Builders<Rental>.Filter.Eq(r => r.Id, id);
        return await _context.Rentals.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        _logger.LogDebug("Getting all rentals");
        return await _context.Rentals.Find(Builders<Rental>.Filter.Empty).ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByDeliveryDriverIdAsync(Guid deliveryDriverId)
    {
        _logger.LogDebug("Getting rentals by delivery driver ID: {DeliveryDriverId}", deliveryDriverId);
        var filter = Builders<Rental>.Filter.Eq(r => r.DeliveryDriverId, deliveryDriverId);
        return await _context.Rentals.Find(filter).ToListAsync();
    }

    public async Task<Rental> CreateAsync(Rental rental)
    {
        _logger.LogInformation("Creating rental for motorcycle {MotorcycleId} and driver {DeliveryDriverId}", 
            rental.MotorcycleId, rental.DeliveryDriverId);
        
        await _context.Rentals.InsertOneAsync(rental);
        
        _logger.LogInformation("Rental created successfully with ID: {Id}", rental.Id);
        return rental;
    }

    public async Task<Rental> UpdateAsync(Rental rental)
    {
        _logger.LogInformation("Updating rental with ID: {Id}", rental.Id);
        
        rental.UpdatedAt = DateTime.UtcNow;
        var filter = Builders<Rental>.Filter.Eq(r => r.Id, rental.Id);
        await _context.Rentals.ReplaceOneAsync(filter, rental);
        
        _logger.LogInformation("Rental updated successfully with ID: {Id}", rental.Id);
        return rental;
    }
}

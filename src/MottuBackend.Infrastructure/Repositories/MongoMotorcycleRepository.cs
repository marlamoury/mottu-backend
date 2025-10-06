using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using MottuBackend.Application.Interfaces;
using MottuBackend.Domain.Entities;
using MottuBackend.Infrastructure.Data;

namespace MottuBackend.Infrastructure.Repositories;

public class MongoMotorcycleRepository : IMotorcycleRepository
{
    private readonly MongoDbContext _context;
    private readonly ILogger<MongoMotorcycleRepository> _logger;

    public MongoMotorcycleRepository(MongoDbContext context, ILogger<MongoMotorcycleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Motorcycle?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Getting motorcycle by ID: {Id}", id);
        var filter = Builders<Motorcycle>.Filter.Eq(m => m.Id, id);
        return await _context.Motorcycles.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate)
    {
        _logger.LogDebug("Getting motorcycle by license plate: {LicensePlate}", licensePlate);
        var filter = Builders<Motorcycle>.Filter.Eq(m => m.LicensePlate, licensePlate);
        return await _context.Motorcycles.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Motorcycle>> GetAllAsync(string? licensePlate = null)
    {
        _logger.LogDebug("Getting all motorcycles with license plate filter: {LicensePlate}", licensePlate);
        
        var filter = licensePlate != null 
            ? Builders<Motorcycle>.Filter.Regex(m => m.LicensePlate, licensePlate)
            : Builders<Motorcycle>.Filter.Empty;
            
        return await _context.Motorcycles.Find(filter).ToListAsync();
    }

    public async Task<Motorcycle> CreateAsync(Motorcycle motorcycle)
    {
        _logger.LogInformation("Creating motorcycle with license plate: {LicensePlate}", motorcycle.LicensePlate);
        
        await _context.Motorcycles.InsertOneAsync(motorcycle);
        
        _logger.LogInformation("Motorcycle created successfully with ID: {Id}", motorcycle.Id);
        return motorcycle;
    }

    public async Task<Motorcycle> UpdateAsync(Motorcycle motorcycle)
    {
        _logger.LogInformation("Updating motorcycle with ID: {Id}", motorcycle.Id);
        
        motorcycle.UpdatedAt = DateTime.UtcNow;
        var filter = Builders<Motorcycle>.Filter.Eq(m => m.Id, motorcycle.Id);
        await _context.Motorcycles.ReplaceOneAsync(filter, motorcycle);
        
        _logger.LogInformation("Motorcycle updated successfully with ID: {Id}", motorcycle.Id);
        return motorcycle;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting motorcycle with ID: {Id}", id);
        
        var filter = Builders<Motorcycle>.Filter.Eq(m => m.Id, id);
        var result = await _context.Motorcycles.DeleteOneAsync(filter);
        
        _logger.LogInformation("Motorcycle deleted successfully with ID: {Id}", id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> HasActiveRentalsAsync(Guid motorcycleId)
    {
        _logger.LogDebug("Checking if motorcycle {MotorcycleId} has active rentals", motorcycleId);
        
        var filter = Builders<Rental>.Filter.Eq(r => r.MotorcycleId, motorcycleId);
        var count = await _context.Rentals.CountDocumentsAsync(filter);
        
        return count > 0;
    }
}

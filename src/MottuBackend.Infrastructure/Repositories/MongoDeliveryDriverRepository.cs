using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using MottuBackend.Application.Interfaces;
using MottuBackend.Domain.Entities;
using MottuBackend.Infrastructure.Data;

namespace MottuBackend.Infrastructure.Repositories;

public class MongoDeliveryDriverRepository : IDeliveryDriverRepository
{
    private readonly MongoDbContext _context;
    private readonly ILogger<MongoDeliveryDriverRepository> _logger;

    public MongoDeliveryDriverRepository(MongoDbContext context, ILogger<MongoDeliveryDriverRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DeliveryDriver?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("Getting delivery driver by ID: {Id}", id);
        var filter = Builders<DeliveryDriver>.Filter.Eq(d => d.Id, id);
        return await _context.DeliveryDrivers.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<DeliveryDriver?> GetByCnpjAsync(string cnpj)
    {
        _logger.LogDebug("Getting delivery driver by CNPJ: {Cnpj}", cnpj);
        var filter = Builders<DeliveryDriver>.Filter.Eq(d => d.Cnpj, cnpj);
        return await _context.DeliveryDrivers.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<DeliveryDriver?> GetByLicenseNumberAsync(string licenseNumber)
    {
        _logger.LogDebug("Getting delivery driver by license number: {LicenseNumber}", licenseNumber);
        var filter = Builders<DeliveryDriver>.Filter.Eq(d => d.LicenseNumber, licenseNumber);
        return await _context.DeliveryDrivers.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<DeliveryDriver>> GetAllAsync()
    {
        _logger.LogDebug("Getting all delivery drivers");
        return await _context.DeliveryDrivers.Find(Builders<DeliveryDriver>.Filter.Empty).ToListAsync();
    }

    public async Task<DeliveryDriver> CreateAsync(DeliveryDriver deliveryDriver)
    {
        _logger.LogInformation("Creating delivery driver with CNPJ: {Cnpj}", deliveryDriver.Cnpj);
        
        await _context.DeliveryDrivers.InsertOneAsync(deliveryDriver);
        
        _logger.LogInformation("Delivery driver created successfully with ID: {Id}", deliveryDriver.Id);
        return deliveryDriver;
    }

    public async Task<DeliveryDriver> UpdateAsync(DeliveryDriver deliveryDriver)
    {
        _logger.LogInformation("Updating delivery driver with ID: {Id}", deliveryDriver.Id);
        
        deliveryDriver.UpdatedAt = DateTime.UtcNow;
        var filter = Builders<DeliveryDriver>.Filter.Eq(d => d.Id, deliveryDriver.Id);
        await _context.DeliveryDrivers.ReplaceOneAsync(filter, deliveryDriver);
        
        _logger.LogInformation("Delivery driver updated successfully with ID: {Id}", deliveryDriver.Id);
        return deliveryDriver;
    }
}

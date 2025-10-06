using MongoDB.Driver;
using MottuBackend.Domain.Entities;

namespace MottuBackend.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<Motorcycle> Motorcycles => _database.GetCollection<Motorcycle>("motorcycles");
    public IMongoCollection<DeliveryDriver> DeliveryDrivers => _database.GetCollection<DeliveryDriver>("delivery_drivers");
    public IMongoCollection<Rental> Rentals => _database.GetCollection<Rental>("rentals");
    public IMongoCollection<MotorcycleNotification> MotorcycleNotifications => _database.GetCollection<MotorcycleNotification>("motorcycle_notifications");
}

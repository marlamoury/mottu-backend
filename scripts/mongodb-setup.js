// MongoDB Setup Script for Mottu Backend
// Run this script in MongoDB shell or MongoDB Compass

// Create database
use mottu_backend;

// Create collections with indexes
db.createCollection("motorcycles");
db.createCollection("delivery_drivers");
db.createCollection("rentals");
db.createCollection("motorcycle_notifications");

// Create indexes for motorcycles
db.motorcycles.createIndex({ "licensePlate": 1 }, { unique: true });
db.motorcycles.createIndex({ "identifier": 1 });
db.motorcycles.createIndex({ "year": 1 });
db.motorcycles.createIndex({ "model": 1 });

// Create indexes for delivery_drivers
db.delivery_drivers.createIndex({ "cnpj": 1 }, { unique: true });
db.delivery_drivers.createIndex({ "licenseNumber": 1 }, { unique: true });
db.delivery_drivers.createIndex({ "identifier": 1 });
db.delivery_drivers.createIndex({ "name": 1 });

// Create indexes for rentals
db.rentals.createIndex({ "motorcycleId": 1 });
db.rentals.createIndex({ "deliveryDriverId": 1 });
db.rentals.createIndex({ "startDate": 1 });
db.rentals.createIndex({ "endDate": 1 });
db.rentals.createIndex({ "createdAt": 1 });

// Create indexes for motorcycle_notifications
db.motorcycle_notifications.createIndex({ "motorcycleId": 1 });
db.motorcycle_notifications.createIndex({ "createdAt": 1 });

// Insert sample data
db.motorcycles.insertMany([
    {
        _id: ObjectId(),
        id: UUID(),
        identifier: "MOT001",
        year: 2024,
        model: "Honda CB 600F",
        licensePlate: "ABC-1234",
        createdAt: new Date(),
        updatedAt: null
    },
    {
        _id: ObjectId(),
        id: UUID(),
        identifier: "MOT002",
        year: 2023,
        model: "Yamaha FZ25",
        licensePlate: "DEF-5678",
        createdAt: new Date(),
        updatedAt: null
    }
]);

db.delivery_drivers.insertMany([
    {
        _id: ObjectId(),
        id: UUID(),
        identifier: "DRV001",
        name: "João Silva",
        cnpj: "12345678901234",
        birthDate: new Date("1990-01-01"),
        licenseNumber: "12345678901",
        licenseType: "A",
        licenseImageUrl: null,
        createdAt: new Date(),
        updatedAt: null
    },
    {
        _id: ObjectId(),
        id: UUID(),
        identifier: "DRV002",
        name: "Maria Santos",
        cnpj: "98765432109876",
        birthDate: new Date("1985-05-15"),
        licenseNumber: "98765432109",
        licenseType: "A+B",
        licenseImageUrl: null,
        createdAt: new Date(),
        updatedAt: null
    }
]);

print("MongoDB setup completed successfully!");
print("Database: mottu_backend");
print("Collections created: motorcycles, delivery_drivers, rentals, motorcycle_notifications");
print("Indexes created for optimal performance");
print("Sample data inserted");

# Mottu Backend API

A comprehensive motorcycle rental management system built with .NET 8, MongoDB, and RabbitMQ.

## 🚀 Features

- **Motorcycle Management**: CRUD operations for motorcycles
- **Delivery Driver Management**: Driver registration with CNH image upload
- **Rental System**: Motorcycle rental with multiple plans (7, 15, 30, 45, 50 days)
- **Message Queue**: RabbitMQ integration for event-driven architecture
- **File Storage**: Local file storage for CNH images
- **Database**: MongoDB for primary storage with SQL Server compatibility
- **Docker Support**: Full containerization with Docker Compose

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   API Layer     │    │ Application     │    │   Domain        │
│   (Controllers)  │◄──►│   (Services)    │◄──►│   (Entities)    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│ Infrastructure  │    │   MongoDB       │    │   RabbitMQ       │
│   (Repositories)│    │   (Database)    │    │   (Messaging)    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🛠️ Technology Stack

- **.NET 8.0** - Backend framework
- **MongoDB** - Primary database
- **RabbitMQ** - Message broker
- **Docker** - Containerization
- **AutoMapper** - Object mapping
- **Serilog** - Logging
- **FluentValidation** - Input validation

## 📋 Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose
- Git

## 🚀 Quick Start

### Development Environment

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd mottu-backend
   ```

2. **Start development services**
   ```bash
   chmod +x scripts/start-dev.sh
   ./scripts/start-dev.sh
   ```

3. **Run the API**
   ```bash
   dotnet run --project src/MottuBackend.API
   ```

### Production Environment

1. **Start all services with Docker**
   ```bash
   chmod +x scripts/start-prod.sh
   ./scripts/start-prod.sh
   ```

## 🐳 Docker Services

| Service | Port | Description |
|---------|------|-------------|
| API | 8080 | Main application |
| MongoDB | 27017 | Database |
| MongoDB UI | 8081 | Database management |
| RabbitMQ | 5672 | Message broker |
| RabbitMQ UI | 15672 | Message broker management |

## 📚 API Documentation

### Base URL
- Development: `http://localhost:5000`
- Production: `http://localhost:8080`

### Endpoints

#### Motorcycles
- `GET /api/motorcycles` - Get all motorcycles
- `GET /api/motorcycles/{id}` - Get motorcycle by ID
- `POST /api/motorcycles` - Create motorcycle
- `PUT /api/motorcycles/{id}` - Update motorcycle
- `DELETE /api/motorcycles/{id}` - Delete motorcycle

#### Delivery Drivers
- `GET /api/deliverydrivers` - Get all drivers
- `GET /api/deliverydrivers/{id}` - Get driver by ID
- `POST /api/deliverydrivers` - Create driver
- `POST /api/deliverydrivers/{id}/license-image` - Upload CNH image

#### Rentals
- `GET /api/rentals` - Get all rentals
- `GET /api/rentals/{id}` - Get rental by ID
- `POST /api/rentals` - Create rental
- `POST /api/rentals/{id}/calculate-return` - Calculate return costs
- `POST /api/rentals/{id}/return` - Return motorcycle

## 💾 Database Schema

### MongoDB Collections

#### Motorcycles
```json
{
  "_id": "ObjectId",
  "id": "UUID",
  "identifier": "string",
  "year": "number",
  "model": "string",
  "licensePlate": "string",
  "createdAt": "DateTime",
  "updatedAt": "DateTime"
}
```

#### Delivery Drivers
```json
{
  "_id": "ObjectId",
  "id": "UUID",
  "identifier": "string",
  "name": "string",
  "cnpj": "string",
  "birthDate": "DateTime",
  "licenseNumber": "string",
  "licenseType": "string",
  "licenseImageUrl": "string",
  "createdAt": "DateTime",
  "updatedAt": "DateTime"
}
```

#### Rentals
```json
{
  "_id": "ObjectId",
  "id": "UUID",
  "motorcycleId": "UUID",
  "deliveryDriverId": "UUID",
  "startDate": "DateTime",
  "endDate": "DateTime",
  "expectedEndDate": "DateTime",
  "planDays": "number",
  "dailyRate": "decimal",
  "totalAmount": "decimal",
  "fineAmount": "decimal",
  "additionalAmount": "decimal",
  "createdAt": "DateTime",
  "updatedAt": "DateTime"
}
```

## 🔄 Message Queue Events

### Motorcycle Registered Event
```json
{
  "motorcycleId": "UUID",
  "identifier": "string",
  "year": "number",
  "model": "string",
  "licensePlate": "string",
  "timestamp": "DateTime"
}
```

## 📁 Project Structure

```
mottu-backend/
├── src/
│   ├── MottuBackend.API/           # API Layer
│   ├── MottuBackend.Application/   # Application Layer
│   ├── MottuBackend.Domain/        # Domain Layer
│   └── MottuBackend.Infrastructure/ # Infrastructure Layer
├── tests/
│   └── MottuBackend.Tests/         # Unit Tests
├── scripts/                        # Setup Scripts
├── docker-compose.yml             # Production Docker Compose
├── docker-compose.dev.yml         # Development Docker Compose
└── Dockerfile                     # API Dockerfile
```

## 🧪 Testing

### Run Unit Tests
```bash
dotnet test
```

### Run with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Monitoring and Logging

- **Health Checks**: `/health` endpoint
- **Logging**: Serilog with console and file sinks
- **Metrics**: Built-in .NET metrics

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment | `Production` |
| `ConnectionStrings__MongoDB` | MongoDB connection | `mongodb://localhost:27017` |
| `MongoDB__DatabaseName` | Database name | `mottu_backend` |
| `FileStorage__BasePath` | File storage path | `/app/uploads` |
| `RabbitMQ__HostName` | RabbitMQ host | `localhost` |
| `RabbitMQ__Port` | RabbitMQ port | `5672` |

## 🚀 Deployment

### Production Deployment

1. **Build and start services**
   ```bash
   docker-compose up -d --build
   ```

2. **Verify services**
   ```bash
   docker-compose ps
   ```

3. **Check logs**
   ```bash
   docker-compose logs -f mottu-api
   ```

### Scaling

```bash
# Scale API instances
docker-compose up -d --scale mottu-api=3
```

## 🔒 Security

- Input validation with FluentValidation
- File upload restrictions (PNG, BMP only)
- CNH image validation
- License plate uniqueness validation

## 📈 Performance

- MongoDB indexes for optimal queries
- Connection pooling
- Async/await patterns throughout
- Efficient message queue processing

## 🐛 Troubleshooting

### Common Issues

1. **MongoDB Connection Failed**
   ```bash
   docker logs mottu-mongodb
   ```

2. **RabbitMQ Connection Failed**
   ```bash
   docker logs mottu-rabbitmq
   ```

3. **API Not Starting**
   ```bash
   docker logs mottu-api
   ```

### Health Checks

- API: `http://localhost:8080/health`
- MongoDB: `docker exec mottu-mongodb mongosh --eval "db.adminCommand('ping')"`
- RabbitMQ: `docker exec mottu-rabbitmq rabbitmq-diagnostics ping`

## 📝 License

This project is licensed under the MIT License.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📞 Support

For support, please open an issue in the repository or contact the development team.
# Development Guide

## Overview

This guide covers setting up the development environment for the Mottu Backend API.

## Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose
- Git
- IDE (Visual Studio, VS Code, or Rider)

## Development Setup

### 1. Clone Repository

```bash
git clone <repository-url>
cd mottu-backend
```

### 2. Start Development Services

```bash
chmod +x scripts/start-dev.sh
./scripts/start-dev.sh
```

This will start:
- MongoDB on port 27017
- MongoDB UI on port 8081
- RabbitMQ on port 5672
- RabbitMQ UI on port 15672

### 3. Run the API

```bash
dotnet run --project src/MottuBackend.API
```

The API will be available at `http://localhost:5000`

## Project Structure

```
mottu-backend/
├── src/
│   ├── MottuBackend.API/           # API Layer
│   │   ├── Controllers/            # API Controllers
│   │   ├── Extensions/            # Service Extensions
│   │   ├── Mapping/               # AutoMapper Profiles
│   │   └── Program.cs             # Application Entry Point
│   ├── MottuBackend.Application/   # Application Layer
│   │   ├── DTOs/                  # Data Transfer Objects
│   │   ├── Interfaces/            # Service Interfaces
│   │   └── Services/              # Business Logic
│   ├── MottuBackend.Domain/        # Domain Layer
│   │   └── Entities/              # Domain Entities
│   └── MottuBackend.Infrastructure/ # Infrastructure Layer
│       ├── Data/                  # Database Contexts
│       ├── Repositories/          # Data Access
│       └── Services/              # External Services
├── tests/
│   └── MottuBackend.Tests/         # Unit Tests
├── scripts/                        # Setup Scripts
└── docs/                          # Documentation
```

## Development Workflow

### 1. Feature Development

1. **Create feature branch**
   ```bash
   git checkout -b feature/motorcycle-crud
   ```

2. **Make changes**
   - Add new entities in `Domain/Entities`
   - Create DTOs in `Application/DTOs`
   - Implement services in `Application/Services`
   - Add controllers in `API/Controllers`

3. **Write tests**
   ```bash
   dotnet test
   ```

4. **Commit changes**
   ```bash
   git add .
   git commit -m "Add motorcycle CRUD operations"
   ```

### 2. Testing

#### Unit Tests
```bash
dotnet test
```

#### Integration Tests
```bash
dotnet test --filter Category=Integration
```

#### Test Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### 3. Code Quality

#### Linting
```bash
dotnet format
```

#### Static Analysis
```bash
dotnet build --verbosity normal
```

## Database Development

### MongoDB

#### Connect to MongoDB
```bash
docker exec -it mottu-mongodb-dev mongosh
```

#### View Collections
```javascript
use mottu_backend
show collections
```

#### Sample Queries
```javascript
// Get all motorcycles
db.motorcycles.find()

// Get motorcycles by year
db.motorcycles.find({year: 2024})

// Get active rentals
db.rentals.find({endDate: {$gt: new Date()}})
```

### SQL Server (Compatibility)

The project also includes SQL Server support for compatibility:

```bash
# Connect to SQL Server
docker exec -it mottu-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd
```

## Message Queue Development

### RabbitMQ

#### Connect to RabbitMQ
```bash
docker exec -it mottu-rabbitmq-dev rabbitmqctl
```

#### View Queues
```bash
docker exec mottu-rabbitmq-dev rabbitmqctl list_queues
```

#### Publish Test Message
```bash
docker exec mottu-rabbitmq-dev rabbitmqadmin publish exchange=motorcycle.events routing_key=motorcycle.registered payload='{"test": "message"}'
```

## API Development

### Swagger Documentation

Access Swagger UI at `http://localhost:5000/swagger`

### Testing Endpoints

#### Using curl
```bash
# Get all motorcycles
curl http://localhost:5000/api/motorcycles

# Create motorcycle
curl -X POST http://localhost:5000/api/motorcycles \
  -H "Content-Type: application/json" \
  -d '{"identifier":"MOT001","year":2024,"model":"Honda CB 600F","licensePlate":"ABC-1234"}'
```

#### Using Postman

Import the API collection from `docs/postman-collection.json`

## Debugging

### Visual Studio Code

1. **Install extensions**
   - C# Dev Kit
   - .NET Core Test Explorer
   - Docker

2. **Configure launch.json**
   ```json
   {
     "version": "0.2.0",
     "configurations": [
       {
         "name": "Launch API",
         "type": "coreclr",
         "request": "launch",
         "preLaunchTask": "build",
         "program": "${workspaceFolder}/src/MottuBackend.API/bin/Debug/net8.0/MottuBackend.API.dll",
         "args": [],
         "cwd": "${workspaceFolder}/src/MottuBackend.API",
         "console": "internalConsole",
         "stopAtEntry": false
       }
     ]
   }
   ```

### Visual Studio

1. **Set startup project**
   - Right-click on `MottuBackend.API`
   - Select "Set as Startup Project"

2. **Configure debugging**
   - Go to Project Properties
   - Set Launch Profile to "Development"

## Performance Testing

### Load Testing

```bash
# Install Apache Bench
sudo apt-get install apache2-utils

# Test API endpoints
ab -n 1000 -c 10 http://localhost:5000/api/motorcycles
```

### Memory Profiling

```bash
# Install dotnet-counters
dotnet tool install --global dotnet-counters

# Monitor API
dotnet-counters monitor --process-id <pid>
```

## Common Issues

### 1. Port Conflicts

If ports are already in use:

```bash
# Check port usage
netstat -tulpn | grep :5000

# Kill process
sudo kill -9 <pid>
```

### 2. Database Connection Issues

```bash
# Check MongoDB status
docker exec mottu-mongodb-dev mongosh --eval "db.adminCommand('ping')"

# Check RabbitMQ status
docker exec mottu-rabbitmq-dev rabbitmq-diagnostics ping
```

### 3. Build Issues

```bash
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Rebuild
dotnet build
```

## Best Practices

### 1. Code Organization

- Keep controllers thin
- Put business logic in services
- Use DTOs for data transfer
- Implement proper error handling

### 2. Testing

- Write unit tests for all services
- Test edge cases
- Mock external dependencies
- Maintain good test coverage

### 3. Documentation

- Document public APIs
- Add XML comments
- Keep README updated
- Document configuration options

### 4. Git Workflow

- Use meaningful commit messages
- Create feature branches
- Review code before merging
- Keep commits atomic

## Contributing

### 1. Fork Repository

```bash
git clone <your-fork-url>
cd mottu-backend
```

### 2. Create Feature Branch

```bash
git checkout -b feature/your-feature
```

### 3. Make Changes

- Follow coding standards
- Add tests
- Update documentation

### 4. Submit Pull Request

- Describe changes
- Link to issues
- Request review

## Resources

- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/core/)
- [MongoDB C# Driver](https://docs.mongodb.com/drivers/csharp/)
- [RabbitMQ .NET Client](https://www.rabbitmq.com/dotnet.html)
- [Docker Documentation](https://docs.docker.com/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

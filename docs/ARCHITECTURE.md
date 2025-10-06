# Architecture Documentation

## Overview

The Mottu Backend API follows a clean architecture pattern with clear separation of concerns, making it maintainable, testable, and scalable.

## Architecture Principles

- **Separation of Concerns**: Each layer has a specific responsibility
- **Dependency Inversion**: High-level modules don't depend on low-level modules
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Client Layer                             │
│  (Web App, Mobile App, Postman, etc.)                          │
└─────────────────────┬───────────────────────────────────────────┘
                      │ HTTP/HTTPS
                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                        API Layer                                │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐ │
│  │   Controllers  │  │   Middleware  │  │   Filters       │ │
│  │   (REST API)   │  │   (Auth, Log) │  │   (Validation)  │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘ │
└─────────────────────┬───────────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Application Layer                            │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐ │
│  │    Services     │  │      DTOs       │  │   Interfaces    │ │
│  │  (Business      │  │  (Data Transfer │  │  (Contracts)    │ │
│  │   Logic)        │  │   Objects)      │  │                │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘ │
└─────────────────────┬───────────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Domain Layer                               │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐ │
│  │    Entities     │  │    Value       │  │   Domain       │ │
│  │  (Business      │  │   Objects      │  │   Services     │ │
│  │   Models)       │  │                │  │                │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘ │
└─────────────────────┬───────────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                          │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐ │
│  │  Repositories   │  │   External     │  │   Cross-Cutting │ │
│  │  (Data Access)  │  │   Services     │  │   Concerns      │ │
│  │                 │  │  (RabbitMQ,   │  │   (Logging,     │ │
│  │                 │  │   File Storage)│  │   Caching)     │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘ │
└─────────────────────┬───────────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Data Layer                                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐ │
│  │    MongoDB      │  │   SQL Server   │  │   File System   │ │
│  │  (Primary DB)   │  │  (Compatibility│  │   (CNH Images) │ │
│  │                 │  │   Layer)       │  │                │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

## Layer Details

### API Layer

**Responsibility**: Handle HTTP requests and responses

**Components**:
- **Controllers**: Handle HTTP requests
- **Middleware**: Cross-cutting concerns (logging, authentication)
- **Filters**: Request/response processing
- **Configuration**: API settings and routing

**Key Files**:
- `Controllers/MotorcyclesController.cs`
- `Controllers/DeliveryDriversController.cs`
- `Controllers/RentalsController.cs`
- `Program.cs`

### Application Layer

**Responsibility**: Business logic and use cases

**Components**:
- **Services**: Business logic implementation
- **DTOs**: Data transfer objects
- **Interfaces**: Service contracts
- **Mappers**: Object-to-object mapping

**Key Files**:
- `Services/MotorcycleService.cs`
- `Services/DeliveryDriverService.cs`
- `Services/RentalService.cs`
- `DTOs/`

### Domain Layer

**Responsibility**: Core business entities and rules

**Components**:
- **Entities**: Business models
- **Value Objects**: Immutable objects
- **Domain Services**: Business logic
- **Enums**: Business constants

**Key Files**:
- `Entities/Motorcycle.cs`
- `Entities/DeliveryDriver.cs`
- `Entities/Rental.cs`

### Infrastructure Layer

**Responsibility**: External concerns and data access

**Components**:
- **Repositories**: Data access implementation
- **External Services**: Third-party integrations
- **Configuration**: Infrastructure settings
- **Cross-cutting Concerns**: Logging, caching

**Key Files**:
- `Repositories/MongoMotorcycleRepository.cs`
- `Services/MessageService.cs`
- `Services/FileStorageService.cs`

## Data Flow

### 1. Request Flow

```
Client → Controller → Service → Repository → Database
```

### 2. Response Flow

```
Database → Repository → Service → Controller → Client
```

### 3. Event Flow

```
Service → Message Queue → Event Handler → Database
```

## Database Design

### MongoDB Collections

#### Motorcycles Collection
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

#### Delivery Drivers Collection
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

#### Rentals Collection
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

### Indexes

#### Motorcycles
- `licensePlate` (unique)
- `identifier`
- `year`
- `model`

#### Delivery Drivers
- `cnpj` (unique)
- `licenseNumber` (unique)
- `identifier`
- `name`

#### Rentals
- `motorcycleId`
- `deliveryDriverId`
- `startDate`
- `endDate`
- `createdAt`

## Message Queue Architecture

### RabbitMQ Setup

#### Exchanges
- **motorcycle.events**: Topic exchange for motorcycle events

#### Queues
- **motorcycle.registered**: Queue for motorcycle registration events

#### Routing Keys
- **motorcycle.registered**: Motorcycle registration events

### Event Flow

```
Motorcycle Created → Publish Event → Queue → Consumer → Store Notification
```

## Security Architecture

### Authentication
- Currently no authentication (development)
- Future: JWT or OAuth2

### Authorization
- Role-based access control (future)
- API key authentication (future)

### Data Protection
- Input validation with FluentValidation
- File upload restrictions
- SQL injection prevention
- XSS protection

## Performance Considerations

### Caching Strategy
- In-memory caching for frequently accessed data
- Redis for distributed caching (future)

### Database Optimization
- MongoDB indexes for query performance
- Connection pooling
- Async/await patterns

### Message Queue
- Persistent messages
- Dead letter queues
- Message acknowledgment

## Scalability

### Horizontal Scaling
- Stateless API design
- Load balancer ready
- Database sharding (future)

### Vertical Scaling
- Resource monitoring
- Performance profiling
- Memory optimization

## Monitoring and Observability

### Logging
- Structured logging with Serilog
- Log levels: Debug, Info, Warning, Error
- Log aggregation (future)

### Metrics
- Application metrics
- Database metrics
- Message queue metrics

### Health Checks
- API health endpoint
- Database connectivity
- Message queue connectivity

## Deployment Architecture

### Containerization
- Docker containers for all services
- Docker Compose for orchestration
- Health checks for reliability

### Service Discovery
- Docker networking
- Environment-based configuration
- Service dependencies

## Future Enhancements

### Microservices
- Split into domain services
- API Gateway
- Service mesh

### Event Sourcing
- Event store
- CQRS pattern
- Event replay

### Advanced Features
- Real-time notifications
- WebSocket support
- GraphQL API

## Technology Decisions

### Why MongoDB?
- Document-based storage
- Flexible schema
- Horizontal scaling
- Rich query capabilities

### Why RabbitMQ?
- Reliable message delivery
- Message routing
- Dead letter queues
- Management UI

### Why .NET 8?
- High performance
- Cross-platform
- Rich ecosystem
- Enterprise support

## Conclusion

The Mottu Backend API follows clean architecture principles with clear separation of concerns, making it maintainable, testable, and scalable. The architecture supports both current requirements and future enhancements while maintaining high performance and reliability.

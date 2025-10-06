# Mottu Backend API Documentation

## Overview

The Mottu Backend API provides a comprehensive motorcycle rental management system with support for motorcycles, delivery drivers, and rental operations.

## Base URL

- **Development**: `http://localhost:5000`
- **Production**: `http://localhost:8080`

## Authentication

Currently, the API does not require authentication. In a production environment, consider implementing JWT or OAuth2 authentication.

## Content Types

- **Request**: `application/json`
- **Response**: `application/json`
- **File Upload**: `multipart/form-data`

## Error Responses

All error responses follow this format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "0HMQ8VQJQJQJQ",
  "errors": {
    "field": ["Error message"]
  }
}
```

## Endpoints

### Motorcycles

#### Get All Motorcycles
```http
GET /api/motorcycles?licensePlate={plate}
```

**Query Parameters:**
- `licensePlate` (optional): Filter by license plate

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "identifier": "MOT001",
    "year": 2024,
    "model": "Honda CB 600F",
    "licensePlate": "ABC-1234",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": null
  }
]
```

#### Get Motorcycle by ID
```http
GET /api/motorcycles/{id}
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "identifier": "MOT001",
  "year": 2024,
  "model": "Honda CB 600F",
  "licensePlate": "ABC-1234",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": null
}
```

#### Create Motorcycle
```http
POST /api/motorcycles
```

**Request Body:**
```json
{
  "identifier": "MOT001",
  "year": 2024,
  "model": "Honda CB 600F",
  "licensePlate": "ABC-1234"
}
```

**Response:** `201 Created`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "identifier": "MOT001",
  "year": 2024,
  "model": "Honda CB 600F",
  "licensePlate": "ABC-1234",
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": null
}
```

#### Update Motorcycle
```http
PUT /api/motorcycles/{id}
```

**Request Body:**
```json
{
  "licensePlate": "XYZ-9876"
}
```

**Response:** `200 OK`

#### Delete Motorcycle
```http
DELETE /api/motorcycles/{id}
```

**Response:** `204 No Content`

### Delivery Drivers

#### Get All Delivery Drivers
```http
GET /api/deliverydrivers
```

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "identifier": "DRV001",
    "name": "João Silva",
    "cnpj": "12345678901234",
    "birthDate": "1990-01-01T00:00:00Z",
    "licenseNumber": "12345678901",
    "licenseType": "A",
    "licenseImageUrl": null,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": null
  }
]
```

#### Get Delivery Driver by ID
```http
GET /api/deliverydrivers/{id}
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "identifier": "DRV001",
  "name": "João Silva",
  "cnpj": "12345678901234",
  "birthDate": "1990-01-01T00:00:00Z",
  "licenseNumber": "12345678901",
  "licenseType": "A",
  "licenseImageUrl": null,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": null
}
```

#### Create Delivery Driver
```http
POST /api/deliverydrivers
```

**Request Body:**
```json
{
  "identifier": "DRV001",
  "name": "João Silva",
  "cnpj": "12345678901234",
  "birthDate": "1990-01-01T00:00:00Z",
  "licenseNumber": "12345678901",
  "licenseType": "A"
}
```

**Response:** `201 Created`

#### Upload License Image
```http
POST /api/deliverydrivers/{id}/license-image
```

**Request Body:** `multipart/form-data`
- `licenseImage`: File (PNG or BMP only)

**Response:** `200 OK`

### Rentals

#### Get All Rentals
```http
GET /api/rentals
```

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "motorcycleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "deliveryDriverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "startDate": "2024-01-01T00:00:00Z",
    "endDate": "2024-01-08T00:00:00Z",
    "expectedEndDate": "2024-01-08T00:00:00Z",
    "planDays": 7,
    "dailyRate": 30.00,
    "totalAmount": 210.00,
    "fineAmount": null,
    "additionalAmount": null,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": null
  }
]
```

#### Get Rental by ID
```http
GET /api/rentals/{id}
```

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "motorcycleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "deliveryDriverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-01-08T00:00:00Z",
  "expectedEndDate": "2024-01-08T00:00:00Z",
  "planDays": 7,
  "dailyRate": 30.00,
  "totalAmount": 210.00,
  "fineAmount": null,
  "additionalAmount": null,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": null
}
```

#### Create Rental
```http
POST /api/rentals
```

**Request Body:**
```json
{
  "motorcycleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "deliveryDriverId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "planDays": 7
}
```

**Response:** `201 Created`

#### Calculate Return Costs
```http
POST /api/rentals/{id}/calculate-return
```

**Request Body:**
```json
{
  "returnDate": "2024-01-10T00:00:00Z"
}
```

**Response:**
```json
{
  "totalAmount": 250.00,
  "fineAmount": 40.00,
  "additionalAmount": 0.00,
  "calculationDetails": "Early return by 2 days. Fine: R$ 40.00"
}
```

#### Return Motorcycle
```http
POST /api/rentals/{id}/return
```

**Request Body:**
```json
{
  "returnDate": "2024-01-10T00:00:00Z"
}
```

**Response:** `200 OK`

## Business Rules

### Motorcycle Rental Plans

| Plan Days | Daily Rate | Total Cost |
|-----------|------------|------------|
| 7         | R$ 30.00   | R$ 210.00  |
| 15        | R$ 28.00   | R$ 420.00  |
| 30        | R$ 22.00   | R$ 660.00  |
| 45        | R$ 20.00   | R$ 900.00  |
| 50        | R$ 18.00   | R$ 900.00  |

### Return Calculations

#### Early Return
- **7-day plan**: 20% fine on unused days
- **15-day plan**: 40% fine on unused days
- **Other plans**: No fine

#### Late Return
- **Additional cost**: R$ 50.00 per extra day

### License Types
- **A**: Motorcycle license (required for rentals)
- **B**: Car license
- **A+B**: Both motorcycle and car license

## Error Codes

| Code | Description |
|------|-------------|
| 400  | Bad Request - Invalid input data |
| 404  | Not Found - Resource not found |
| 409  | Conflict - Business rule violation |
| 500  | Internal Server Error |

## Rate Limiting

Currently, no rate limiting is implemented. Consider implementing rate limiting for production use.

## CORS

CORS is configured to allow all origins in development. Configure appropriate CORS policies for production.

## Health Check

```http
GET /health
```

**Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-01T00:00:00Z",
  "services": {
    "mongodb": "Healthy",
    "rabbitmq": "Healthy"
  }
}
```

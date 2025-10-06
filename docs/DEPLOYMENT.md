# Deployment Guide

## Overview

This guide covers deploying the Mottu Backend API in various environments using Docker and Docker Compose.

## Prerequisites

- Docker 20.10+
- Docker Compose 2.0+
- 4GB RAM minimum
- 10GB disk space

## Environment Setup

### Development Environment

1. **Start development services**
   ```bash
   chmod +x scripts/start-dev.sh
   ./scripts/start-dev.sh
   ```

2. **Run the API locally**
   ```bash
   dotnet run --project src/MottuBackend.API
   ```

### Production Environment

1. **Start all services**
   ```bash
   chmod +x scripts/start-prod.sh
   ./scripts/start-prod.sh
   ```

2. **Verify deployment**
   ```bash
   curl http://localhost:8080/health
   ```

## Docker Services

### API Service
- **Image**: Custom .NET 8.0 image
- **Port**: 8080
- **Health Check**: `/health` endpoint
- **Dependencies**: MongoDB, RabbitMQ

### MongoDB
- **Image**: mongo:7.0
- **Port**: 27017
- **Management UI**: 8081
- **Credentials**: admin/password123

### RabbitMQ
- **Image**: rabbitmq:3.12-management
- **Ports**: 5672 (AMQP), 15672 (Management UI)
- **Credentials**: admin/admin123

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment | `Production` |
| `ASPNETCORE_URLS` | API URL | `http://+:8080` |
| `ConnectionStrings__MongoDB` | MongoDB connection | `mongodb://admin:password123@mongodb:27017/` |
| `MongoDB__DatabaseName` | Database name | `mottu_backend` |
| `FileStorage__BasePath` | File storage path | `/app/uploads` |
| `RabbitMQ__HostName` | RabbitMQ host | `rabbitmq` |
| `RabbitMQ__Port` | RabbitMQ port | `5672` |
| `RabbitMQ__UserName` | RabbitMQ username | `admin` |
| `RabbitMQ__Password` | RabbitMQ password | `admin123` |

### Production Configuration

Create a `.env` file for production:

```env
# Database
MONGODB_CONNECTION=mongodb://admin:password123@mongodb:27017/
MONGODB_DATABASE=mottu_backend

# Message Queue
RABBITMQ_HOST=rabbitmq
RABBITMQ_PORT=5672
RABBITMQ_USER=admin
RABBITMQ_PASS=admin123

# File Storage
FILE_STORAGE_PATH=/app/uploads

# API
API_PORT=8080
API_ENVIRONMENT=Production
```

## Scaling

### Horizontal Scaling

Scale the API service:

```bash
docker-compose up -d --scale mottu-api=3
```

### Load Balancing

For production, use a reverse proxy like Nginx:

```nginx
upstream mottu_api {
    server mottu-api_1:8080;
    server mottu-api_2:8080;
    server mottu-api_3:8080;
}

server {
    listen 80;
    location / {
        proxy_pass http://mottu_api;
    }
}
```

## Monitoring

### Health Checks

- **API**: `http://localhost:8080/health`
- **MongoDB**: `docker exec mottu-mongodb mongosh --eval "db.adminCommand('ping')"`
- **RabbitMQ**: `docker exec mottu-rabbitmq rabbitmq-diagnostics ping`

### Logs

View logs for all services:

```bash
docker-compose logs -f
```

View logs for specific service:

```bash
docker-compose logs -f mottu-api
```

### Metrics

The API exposes metrics at `/metrics` endpoint (if configured).

## Backup and Recovery

### MongoDB Backup

```bash
# Create backup
docker exec mottu-mongodb mongodump --out /backup

# Copy backup to host
docker cp mottu-mongodb:/backup ./mongodb-backup

# Restore backup
docker exec mottu-mongodb mongorestore /backup
```

### File Storage Backup

```bash
# Backup uploads
docker cp mottu-api:/app/uploads ./uploads-backup

# Restore uploads
docker cp ./uploads-backup mottu-api:/app/uploads
```

## Security

### Network Security

1. **Use private networks**
   ```yaml
   networks:
     mottu-network:
       driver: bridge
       internal: true
   ```

2. **Limit exposed ports**
   ```yaml
   ports:
     - "8080:8080"  # Only expose necessary ports
   ```

### Data Security

1. **Use secrets for passwords**
   ```yaml
   environment:
     MONGO_INITDB_ROOT_PASSWORD_FILE: /run/secrets/mongo_password
   secrets:
     mongo_password:
       file: ./secrets/mongo_password.txt
   ```

2. **Enable SSL/TLS**
   ```yaml
   volumes:
     - ./ssl:/etc/ssl/certs
   ```

## Troubleshooting

### Common Issues

1. **Services not starting**
   ```bash
   docker-compose logs -f
   ```

2. **Database connection failed**
   ```bash
   docker exec mottu-mongodb mongosh --eval "db.adminCommand('ping')"
   ```

3. **Message queue connection failed**
   ```bash
   docker exec mottu-rabbitmq rabbitmq-diagnostics ping
   ```

4. **API not responding**
   ```bash
   curl -f http://localhost:8080/health
   ```

### Performance Issues

1. **Check resource usage**
   ```bash
   docker stats
   ```

2. **Monitor logs**
   ```bash
   docker-compose logs -f mottu-api
   ```

3. **Check database performance**
   ```bash
   docker exec mottu-mongodb mongosh --eval "db.runCommand({serverStatus: 1})"
   ```

## Maintenance

### Updates

1. **Update images**
   ```bash
   docker-compose pull
   docker-compose up -d
   ```

2. **Rebuild API**
   ```bash
   docker-compose build mottu-api
   docker-compose up -d mottu-api
   ```

### Cleanup

1. **Remove unused containers**
   ```bash
   docker system prune -f
   ```

2. **Remove unused volumes**
   ```bash
   docker volume prune -f
   ```

## Production Checklist

- [ ] Change default passwords
- [ ] Configure SSL/TLS
- [ ] Set up monitoring
- [ ] Configure backups
- [ ] Set up log aggregation
- [ ] Configure health checks
- [ ] Set up alerting
- [ ] Review security settings
- [ ] Test disaster recovery
- [ ] Document runbooks

## Support

For deployment issues:

1. Check logs: `docker-compose logs -f`
2. Verify health: `curl http://localhost:8080/health`
3. Check resources: `docker stats`
4. Review configuration files
5. Contact support team

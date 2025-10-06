#!/bin/bash

# Start production environment
echo "Starting Mottu Backend production environment..."

# Build and start all services
docker-compose up -d --build

# Wait for services to be ready
echo "Waiting for services to be ready..."
sleep 30

# Check if MongoDB is ready
echo "Checking MongoDB connection..."
docker exec mottu-mongodb mongosh --eval "db.adminCommand('ping')" > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ MongoDB is ready"
else
    echo "❌ MongoDB is not ready"
fi

# Check if RabbitMQ is ready
echo "Checking RabbitMQ connection..."
docker exec mottu-rabbitmq rabbitmq-diagnostics ping > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ RabbitMQ is ready"
else
    echo "❌ RabbitMQ is not ready"
fi

# Check if API is ready
echo "Checking API health..."
curl -f http://localhost:8080/health > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ API is ready"
else
    echo "❌ API is not ready"
fi

echo ""
echo "🚀 Production environment is ready!"
echo ""
echo "Services available at:"
echo "  - API: http://localhost:8080"
echo "  - API Health: http://localhost:8080/health"
echo "  - API Swagger: http://localhost:8080/swagger"
echo "  - MongoDB: mongodb://localhost:27017"
echo "  - MongoDB UI: http://localhost:8081 (admin/admin123)"
echo "  - RabbitMQ: amqp://localhost:5672"
echo "  - RabbitMQ UI: http://localhost:15672 (admin/admin123)"
echo ""
echo "To stop the environment:"
echo "  docker-compose down"

#!/bin/bash

# Start development environment
echo "Starting Mottu Backend development environment..."

# Start MongoDB and RabbitMQ
docker-compose -f docker-compose.dev.yml up -d

# Wait for services to be ready
echo "Waiting for services to be ready..."
sleep 10

# Check if MongoDB is ready
echo "Checking MongoDB connection..."
docker exec mottu-mongodb-dev mongosh --eval "db.adminCommand('ping')" > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ MongoDB is ready"
else
    echo "❌ MongoDB is not ready"
fi

# Check if RabbitMQ is ready
echo "Checking RabbitMQ connection..."
docker exec mottu-rabbitmq-dev rabbitmq-diagnostics ping > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ RabbitMQ is ready"
else
    echo "❌ RabbitMQ is not ready"
fi

echo ""
echo "🚀 Development environment is ready!"
echo ""
echo "Services available at:"
echo "  - MongoDB: mongodb://localhost:27017"
echo "  - MongoDB UI: http://localhost:8081 (admin/admin123)"
echo "  - RabbitMQ: amqp://localhost:5672"
echo "  - RabbitMQ UI: http://localhost:15672 (admin/admin123)"
echo ""
echo "To run the API:"
echo "  dotnet run --project src/MottuBackend.API"
echo ""
echo "To stop the environment:"
echo "  docker-compose -f docker-compose.dev.yml down"

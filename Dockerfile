# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/MottuBackend.API/MottuBackend.API.csproj", "src/MottuBackend.API/"]
COPY ["src/MottuBackend.Application/MottuBackend.Application.csproj", "src/MottuBackend.Application/"]
COPY ["src/MottuBackend.Domain/MottuBackend.Domain.csproj", "src/MottuBackend.Domain/"]
COPY ["src/MottuBackend.Infrastructure/MottuBackend.Infrastructure.csproj", "src/MottuBackend.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/MottuBackend.API/MottuBackend.API.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/MottuBackend.API"
RUN dotnet build "MottuBackend.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MottuBackend.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8.0 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create uploads directory
RUN mkdir -p /app/uploads/cnh

# Copy the published application
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "MottuBackend.API.dll"]
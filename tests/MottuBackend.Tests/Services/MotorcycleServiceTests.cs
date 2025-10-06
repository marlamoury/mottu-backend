using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MottuBackend.Application.DTOs;
using MottuBackend.Application.Interfaces;
using MottuBackend.Application.Services;
using MottuBackend.Domain.Entities;
using Xunit;

namespace MottuBackend.Tests.Services;

public class MotorcycleServiceTests
{
    private readonly Mock<IMotorcycleRepository> _repositoryMock;
    private readonly Mock<IMessageService> _messageServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<MotorcycleService>> _loggerMock;
    private readonly MotorcycleService _service;

    public MotorcycleServiceTests()
    {
        _repositoryMock = new Mock<IMotorcycleRepository>();
        _messageServiceMock = new Mock<IMessageService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<MotorcycleService>>();
        
        _service = new MotorcycleService(
            _repositoryMock.Object,
            _messageServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateMotorcycle_WhenValidData()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto
        {
            Identifier = "MOT001",
            Year = 2024,
            Model = "Honda CB 600F",
            LicensePlate = "ABC-1234"
        };

        var motorcycle = new Motorcycle
        {
            Id = Guid.NewGuid(),
            Identifier = createDto.Identifier,
            Year = createDto.Year,
            Model = createDto.Model,
            LicensePlate = createDto.LicensePlate
        };

        var motorcycleDto = new MotorcycleDto
        {
            Id = motorcycle.Id,
            Identifier = motorcycle.Identifier,
            Year = motorcycle.Year,
            Model = motorcycle.Model,
            LicensePlate = motorcycle.LicensePlate
        };

        _repositoryMock.Setup(r => r.GetByLicensePlateAsync(createDto.LicensePlate))
            .ReturnsAsync((Motorcycle?)null);
        _mapperMock.Setup(m => m.Map<Motorcycle>(createDto))
            .Returns(motorcycle);
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Motorcycle>()))
            .ReturnsAsync(motorcycle);
        _mapperMock.Setup(m => m.Map<MotorcycleDto>(motorcycle))
            .Returns(motorcycleDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Identifier.Should().Be(createDto.Identifier);
        result.LicensePlate.Should().Be(createDto.LicensePlate);
        
        _messageServiceMock.Verify(s => s.PublishMotorcycleRegisteredAsync(
            motorcycle.Id, 
            motorcycle.Identifier, 
            motorcycle.Year, 
            motorcycle.Model, 
            motorcycle.LicensePlate), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenLicensePlateExists()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto
        {
            Identifier = "MOT001",
            Year = 2024,
            Model = "Honda CB 600F",
            LicensePlate = "ABC-1234"
        };

        var existingMotorcycle = new Motorcycle
        {
            Id = Guid.NewGuid(),
            LicensePlate = createDto.LicensePlate
        };

        _repositoryMock.Setup(r => r.GetByLicensePlateAsync(createDto.LicensePlate))
            .ReturnsAsync(existingMotorcycle);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMotorcycles()
    {
        // Arrange
        var motorcycles = new List<Motorcycle>
        {
            new() { Id = Guid.NewGuid(), Identifier = "MOT001", LicensePlate = "ABC-1234" },
            new() { Id = Guid.NewGuid(), Identifier = "MOT002", LicensePlate = "DEF-5678" }
        };

        var motorcycleDtos = new List<MotorcycleDto>
        {
            new() { Id = motorcycles[0].Id, Identifier = "MOT001", LicensePlate = "ABC-1234" },
            new() { Id = motorcycles[1].Id, Identifier = "MOT002", LicensePlate = "DEF-5678" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync(null))
            .ReturnsAsync(motorcycles);
        _mapperMock.Setup(m => m.Map<IEnumerable<MotorcycleDto>>(motorcycles))
            .Returns(motorcycleDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMotorcycle_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycle = new Motorcycle { Id = id, Identifier = "MOT001" };
        var motorcycleDto = new MotorcycleDto { Id = id, Identifier = "MOT001" };

        _repositoryMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(motorcycle);
        _mapperMock.Setup(m => m.Map<MotorcycleDto>(motorcycle))
            .Returns(motorcycleDto);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result?.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Motorcycle?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMotorcycle_WhenValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateMotorcycleDto { LicensePlate = "XYZ-9876" };
        var motorcycle = new Motorcycle { Id = id, LicensePlate = "ABC-1234" };
        var updatedMotorcycle = new Motorcycle { Id = id, LicensePlate = updateDto.LicensePlate };
        var motorcycleDto = new MotorcycleDto { Id = id, LicensePlate = updateDto.LicensePlate };

        _repositoryMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(motorcycle);
        _repositoryMock.Setup(r => r.GetByLicensePlateAsync(updateDto.LicensePlate))
            .ReturnsAsync((Motorcycle?)null);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Motorcycle>()))
            .ReturnsAsync(updatedMotorcycle);
        _mapperMock.Setup(m => m.Map<MotorcycleDto>(updatedMotorcycle))
            .Returns(motorcycleDto);

        // Act
        var result = await _service.UpdateAsync(id, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.LicensePlate.Should().Be(updateDto.LicensePlate);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenMotorcycleNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateMotorcycleDto { LicensePlate = "XYZ-9876" };

        _repositoryMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Motorcycle?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(id, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteMotorcycle_WhenNoActiveRentals()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycle = new Motorcycle { Id = id };

        _repositoryMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(motorcycle);
        _repositoryMock.Setup(r => r.HasActiveRentalsAsync(id))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.DeleteAsync(id))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenMotorcycleNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Motorcycle?)null);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenHasActiveRentals()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycle = new Motorcycle { Id = id };

        _repositoryMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(motorcycle);
        _repositoryMock.Setup(r => r.HasActiveRentalsAsync(id))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(id));
    }
}

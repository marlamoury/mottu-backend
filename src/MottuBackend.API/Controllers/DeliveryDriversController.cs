using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MottuBackend.Application.DTOs;
using MottuBackend.Application.Interfaces;

namespace MottuBackend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryDriversController : ControllerBase
{
    private readonly IDeliveryDriverService _deliveryDriverService;
    private readonly ILogger<DeliveryDriversController> _logger;

    public DeliveryDriversController(IDeliveryDriverService deliveryDriverService, ILogger<DeliveryDriversController> logger)
    {
        _deliveryDriverService = deliveryDriverService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryDriverDto>> CreateDeliveryDriver([FromBody] CreateDeliveryDriverDto createDto)
    {
        try
        {
            var deliveryDriver = await _deliveryDriverService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetDeliveryDriver), new { id = deliveryDriver.Id }, deliveryDriver);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Argument error when creating delivery driver: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Operation error when creating delivery driver: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating delivery driver");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryDriverDto>>> GetDeliveryDrivers()
    {
        try
        {
            var deliveryDrivers = await _deliveryDriverService.GetAllAsync();
            return Ok(deliveryDrivers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting delivery drivers");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryDriverDto>> GetDeliveryDriver(Guid id)
    {
        try
        {
            var deliveryDriver = await _deliveryDriverService.GetByIdAsync(id);
            if (deliveryDriver == null)
            {
                return NotFound();
            }
            return Ok(deliveryDriver);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting delivery driver {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/license-image")]
    public async Task<ActionResult<DeliveryDriverDto>> UpdateLicenseImage(Guid id, [FromForm] UpdateLicenseImageDto updateDto)
    {
        try
        {
            var deliveryDriver = await _deliveryDriverService.UpdateLicenseImageAsync(id, updateDto);
            return Ok(deliveryDriver);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Argument error when updating license image: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating license image for delivery driver {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MottuBackend.Application.DTOs;
using MottuBackend.Application.Interfaces;

namespace MottuBackend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MotorcyclesController : ControllerBase
{
    private readonly IMotorcycleService _motorcycleService;
    private readonly ILogger<MotorcyclesController> _logger;

    public MotorcyclesController(IMotorcycleService motorcycleService, ILogger<MotorcyclesController> logger)
    {
        _motorcycleService = motorcycleService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<MotorcycleDto>> CreateMotorcycle([FromBody] CreateMotorcycleDto createDto)
    {
        try
        {
            var motorcycle = await _motorcycleService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetMotorcycle), new { id = motorcycle.Id }, motorcycle);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Operation error when creating motorcycle: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating motorcycle");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MotorcycleDto>>> GetMotorcycles([FromQuery] string? licensePlate = null)
    {
        try
        {
            var motorcycles = await _motorcycleService.GetAllAsync(licensePlate);
            return Ok(motorcycles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting motorcycles");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MotorcycleDto>> GetMotorcycle(Guid id)
    {
        try
        {
            var motorcycle = await _motorcycleService.GetByIdAsync(id);
            if (motorcycle == null)
            {
                return NotFound();
            }
            return Ok(motorcycle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting motorcycle {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MotorcycleDto>> UpdateMotorcycle(Guid id, [FromBody] UpdateMotorcycleDto updateDto)
    {
        try
        {
            var motorcycle = await _motorcycleService.UpdateAsync(id, updateDto);
            return Ok(motorcycle);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Operation error when updating motorcycle: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating motorcycle {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMotorcycle(Guid id)
    {
        try
        {
            var result = await _motorcycleService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Operation error when deleting motorcycle: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting motorcycle {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}

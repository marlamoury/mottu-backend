using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MottuBackend.Application.DTOs;
using MottuBackend.Application.Interfaces;

namespace MottuBackend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;
    private readonly ILogger<RentalsController> _logger;

    public RentalsController(IRentalService rentalService, ILogger<RentalsController> logger)
    {
        _rentalService = rentalService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<RentalDto>> CreateRental([FromBody] CreateRentalDto createDto)
    {
        try
        {
            var rental = await _rentalService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetRental), new { id = rental.Id }, rental);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Argument error when creating rental: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Resource not found when creating rental: {Message}", ex.Message);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Operation error when creating rental: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating rental");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RentalDto>>> GetRentals()
    {
        try
        {
            var rentals = await _rentalService.GetAllAsync();
            return Ok(rentals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rentals");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RentalDto>> GetRental(Guid id)
    {
        try
        {
            var rental = await _rentalService.GetByIdAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            return Ok(rental);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rental {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/calculate-return")]
    public async Task<ActionResult<RentalCalculationDto>> CalculateReturn(Guid id, [FromBody] ReturnRentalDto returnDto)
    {
        try
        {
            var calculation = await _rentalService.CalculateReturnAsync(id, returnDto);
            return Ok(calculation);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating return for rental {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/return")]
    public async Task<ActionResult<RentalDto>> ReturnRental(Guid id, [FromBody] ReturnRentalDto returnDto)
    {
        try
        {
            var rental = await _rentalService.ReturnAsync(id, returnDto);
            return Ok(rental);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error returning rental {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}

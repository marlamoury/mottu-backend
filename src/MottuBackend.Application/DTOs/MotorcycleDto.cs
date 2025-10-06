namespace MottuBackend.Application.DTOs;

public class MotorcycleDto
{
    public Guid Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateMotorcycleDto
{
    public string Identifier { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
}

public class UpdateMotorcycleDto
{
    public string LicensePlate { get; set; } = string.Empty;
}

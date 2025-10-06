namespace MottuBackend.Application.DTOs;

public class RentalDto
{
    public Guid Id { get; set; }
    public Guid MotorcycleId { get; set; }
    public Guid DeliveryDriverId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public int PlanDays { get; set; }
    public decimal DailyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? FineAmount { get; set; }
    public decimal? AdditionalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateRentalDto
{
    public Guid MotorcycleId { get; set; }
    public Guid DeliveryDriverId { get; set; }
    public int PlanDays { get; set; }
}

public class ReturnRentalDto
{
    public DateTime ReturnDate { get; set; }
}

public class RentalCalculationDto
{
    public decimal TotalAmount { get; set; }
    public decimal? FineAmount { get; set; }
    public decimal? AdditionalAmount { get; set; }
    public string CalculationDetails { get; set; } = string.Empty;
}

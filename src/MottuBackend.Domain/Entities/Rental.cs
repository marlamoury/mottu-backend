using System.ComponentModel.DataAnnotations;

namespace MottuBackend.Domain.Entities;

public class Rental
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid MotorcycleId { get; set; }
    
    [Required]
    public Guid DeliveryDriverId { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    public DateTime ExpectedEndDate { get; set; }
    
    [Required]
    public int PlanDays { get; set; }
    
    [Required]
    public decimal DailyRate { get; set; }
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    public decimal? FineAmount { get; set; }
    public decimal? AdditionalAmount { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Motorcycle Motorcycle { get; set; } = null!;
    public virtual DeliveryDriver DeliveryDriver { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;

namespace MottuBackend.Domain.Entities;

public class MotorcycleNotification
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid MotorcycleId { get; set; }
    
    [Required]
    public string Message { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Motorcycle Motorcycle { get; set; } = null!;
}

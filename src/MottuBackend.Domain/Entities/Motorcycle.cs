using System.ComponentModel.DataAnnotations;

namespace MottuBackend.Domain.Entities;

public class Motorcycle
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(10)]
    public string Identifier { get; set; } = string.Empty;
    
    [Required]
    public int Year { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    public string LicensePlate { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}

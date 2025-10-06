using System.ComponentModel.DataAnnotations;

namespace MottuBackend.Domain.Entities;

public class DeliveryDriver
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(10)]
    public string Identifier { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(18)]
    public string Cnpj { get; set; } = string.Empty;
    
    [Required]
    public DateTime BirthDate { get; set; }
    
    [Required]
    [StringLength(20)]
    public string LicenseNumber { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    public string LicenseType { get; set; } = string.Empty; // A, B, or A+B
    
    [StringLength(500)]
    public string? LicenseImageUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}

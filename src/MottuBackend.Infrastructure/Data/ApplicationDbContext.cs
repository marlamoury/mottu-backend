using Microsoft.EntityFrameworkCore;
using MottuBackend.Domain.Entities;

namespace MottuBackend.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Motorcycle> Motorcycles { get; set; }
    public DbSet<DeliveryDriver> DeliveryDrivers { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<MotorcycleNotification> MotorcycleNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Motorcycle configurations
        modelBuilder.Entity<Motorcycle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Identifier).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LicensePlate).IsRequired().HasMaxLength(10);
            entity.HasIndex(e => e.LicensePlate).IsUnique();
        });

        // DeliveryDriver configurations
        modelBuilder.Entity<DeliveryDriver>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Identifier).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cnpj).IsRequired().HasMaxLength(18);
            entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.LicenseType).IsRequired().HasMaxLength(10);
            entity.Property(e => e.LicenseImageUrl).HasMaxLength(500);
            entity.HasIndex(e => e.Cnpj).IsUnique();
            entity.HasIndex(e => e.LicenseNumber).IsUnique();
        });

        // Rental configurations
        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Motorcycle)
                  .WithMany(m => m.Rentals)
                  .HasForeignKey(e => e.MotorcycleId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.DeliveryDriver)
                  .WithMany(d => d.Rentals)
                  .HasForeignKey(e => e.DeliveryDriverId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // MotorcycleNotification configurations
        modelBuilder.Entity<MotorcycleNotification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).IsRequired();
            entity.HasOne(e => e.Motorcycle)
                  .WithMany()
                  .HasForeignKey(e => e.MotorcycleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

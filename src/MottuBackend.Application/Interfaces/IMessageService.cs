namespace MottuBackend.Application.Interfaces;

public interface IMessageService
{
    Task PublishMotorcycleRegisteredAsync(Guid motorcycleId, string identifier, int year, string model, string licensePlate);
    Task ConsumeMotorcycleNotificationsAsync();
}

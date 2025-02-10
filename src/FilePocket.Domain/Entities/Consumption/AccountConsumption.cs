namespace FilePocket.Domain.Entities.Consumption;

public abstract class AccountConsumption
{
    public const string StorageCapacity = "StorageCapacity";
    public const string RateLimit = "RateLimit";

    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; protected set; }
    public bool IsActivated { get; protected set; }

    /// <summary>
    /// Discriminator column to distinguish metric types 
    /// </summary>
    public abstract string MetricType { get; protected set; }
}

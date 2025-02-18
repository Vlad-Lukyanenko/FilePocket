using FilePocket.Domain.Entities.Consumption;

namespace FilePocket.Application.Interfaces.Repositories;

public interface IAccountConsumptionRepository
{
    Task<StorageConsumption?> GetStorageConsumptionAsync(
        Guid userId,
        bool lockChanges,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the given consumption record as updated in the EF Core context.
    /// </summary>
    void Update(AccountConsumption consumption);
}
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain.Entities.Consumption;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.Persistence.Repositories;

internal sealed class AccountConsumptionRepository(FilePocketDbContext context) 
    : RepositoryBase<AccountConsumption>(context), IAccountConsumptionRepository
{
    public async Task<StorageConsumption?> GetStorageConsumptionAsync(
        Guid userId,
        bool lockChanges,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        var accountConsumption = await GetByUserAndTypeAsync(
            userId,
            metricType: AccountConsumption.StorageCapacity,
            lockChanges,
            trackChanges,
            cancellationToken);

        return accountConsumption as StorageConsumption;
    }

    /// <summary>
    /// Retrieves the consumption record for a specific user and metric type.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="metricType">The metric type (e.g. "StorageCapacity" or "RateLimit").</param>
    /// <param name="lockChanges">If lockChanges is true, the record is locked for update.</param>
    /// <param name="trackChanges">Whether to lock the row (using FOR UPDATE) to prevent concurrent updates.</param>
    /// <param name="cancellationToken">To stop the query execution </param>
    private Task<AccountConsumption?> GetByUserAndTypeAsync(
        Guid userId,
        string metricType,
        bool lockChanges,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        if (!lockChanges)
            return FindByCondition(ac => ac.UserId == userId && ac.MetricType == metricType, trackChanges)
                .FirstOrDefaultAsync(cancellationToken);

        const string getAccountConsumptionsByMetricTypeQuery = @"
                SELECT * FROM public.""AccountConsumptions"" 
                WHERE ""UserId"" = {0} AND ""MetricType"" = {1} 
                FOR UPDATE";

        return DbContext.AccountConsumptions
            .FromSqlRaw(getAccountConsumptionsByMetricTypeQuery, userId, metricType)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
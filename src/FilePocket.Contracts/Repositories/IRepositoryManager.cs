using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace FilePocket.Contracts.Repositories;

public interface IRepositoryManager
{
    IPocketRepository Pocket { get; }
    ISharedFileRepository SharedFile { get; }
    IFolderRepository Folder { get; }
    IFileMetadataRepository FileMetadata { get; }
    IAccountConsumptionRepository AccountConsumption { get; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

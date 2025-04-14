using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace FilePocket.Application.Interfaces.Repositories;

public interface IRepositoryManager
{
    IPocketRepository Pocket { get; }
    ISharedFileRepository SharedFile { get; }
    IFolderRepository Folder { get; }
    IFileMetadataRepository FileMetadata { get; }
    IAccountConsumptionRepository AccountConsumption { get; }
    IBookmarkRepository Bookmark { get; }
    IProfileRepository Profile { get; }
    INotesRepository Note { get; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(IDbTransaction transaction, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

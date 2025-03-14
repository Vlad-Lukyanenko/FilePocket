using System.Data;
using System.Data.Common;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace FilePocket.Infrastructure.Persistence.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly FilePocketDbContext _dbContext;
    private readonly Lazy<IPocketRepository> _pocketRepository;
    private readonly Lazy<ISharedFileRepository> _sharedFileRepository;
    private readonly Lazy<IFolderRepository> _folderRepository;
    private readonly Lazy<IFileMetadataRepository> _fileMetadataRepository;
    private readonly Lazy<IAccountConsumptionRepository> _accountConsumptionRepository;
    private readonly Lazy<IBookmarkRepository> _bookmarkRepository;

    public RepositoryManager(FilePocketDbContext dbContext, UserManager<User> userManager, IServiceScopeFactory scopeFactory)
    {
        _dbContext = dbContext;
        _pocketRepository = new Lazy<IPocketRepository>(() => new PocketRepository(dbContext));
        _sharedFileRepository = new Lazy<ISharedFileRepository>(() => new SharedFileRepository(dbContext, userManager));
        _folderRepository = new Lazy<IFolderRepository>(() => new FolderRepository(dbContext, scopeFactory));
        _fileMetadataRepository = new Lazy<IFileMetadataRepository>(() => new FileMetadataRepository(dbContext));
        _accountConsumptionRepository = new Lazy<IAccountConsumptionRepository>(() => new AccountConsumptionRepository(dbContext));
        _bookmarkRepository = new Lazy<IBookmarkRepository>(() => new BookmarkRepository(dbContext));
    }

    public IPocketRepository Pocket => _pocketRepository.Value;
    public ISharedFileRepository SharedFile => _sharedFileRepository.Value;
    public IFolderRepository Folder => _folderRepository.Value;
    public IFileMetadataRepository FileMetadata => _fileMetadataRepository.Value;
    public IAccountConsumptionRepository AccountConsumption => _accountConsumptionRepository.Value;
    public IBookmarkRepository Bookmark => _bookmarkRepository.Value;

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitTransactionAsync(IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        if (transaction is not DbTransaction dbTransaction)
        {
            throw new ArgumentException("Transaction must be of type DbTransaction", nameof(transaction));
        }

        return dbTransaction.CommitAsync(cancellationToken);
    }

    public Task RollbackTransactionAsync(IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        if (transaction is not DbTransaction dbTransaction)
        {
            throw new ArgumentException("Transaction must be of type DbTransaction", nameof(transaction));
        }

        return dbTransaction.RollbackAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

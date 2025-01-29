using FilePocket.Contracts.Repositories;
using FilePocket.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FilePocket.DataAccess.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly FilePocketDbContext _dbContext;
    private readonly Lazy<IPocketRepository> _pocketRepository;
    private readonly Lazy<ISharedFileRepository> _sharedFileRepository;
    private readonly Lazy<IFolderRepository> _folderRepository;
    private readonly Lazy<IFileMetadataRepository> _fileMetadataRepository;

    public RepositoryManager(FilePocketDbContext dbContext, UserManager<User> userManager, IServiceScopeFactory scopeFactory)
    {
        _dbContext = dbContext;
        _pocketRepository = new Lazy<IPocketRepository>(() => new PocketRepository(dbContext));
        _sharedFileRepository = new Lazy<ISharedFileRepository>(() => new SharedFileRepository(dbContext, userManager));
        _folderRepository = new Lazy<IFolderRepository>(() => new FolderRepository(dbContext, scopeFactory));
        _fileMetadataRepository = new Lazy<IFileMetadataRepository>(() => new FileMetadataRepository(dbContext));
    }

    public IPocketRepository Pocket
    {
        get { return _pocketRepository.Value; }
    }

    public ISharedFileRepository SharedFile
    {
        get { return _sharedFileRepository.Value; }
    }

    public IFolderRepository Folder
    {
        get { return _folderRepository.Value; }
    }

    public IFileMetadataRepository FileMetadata
    {
        get { return _fileMetadataRepository.Value; }
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}

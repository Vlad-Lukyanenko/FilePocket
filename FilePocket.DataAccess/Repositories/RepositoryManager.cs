using FilePocket.Contracts.Repositories;

namespace FilePocket.DataAccess.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly FilePocketDbContext _dbContext;
    private readonly Lazy<IStorageRepository> _storageRepository;
    private readonly Lazy<IFileUploadSummaryRepository> _fileUploadSummaryRepository;

    public RepositoryManager(FilePocketDbContext dbContext)
    {
        _dbContext = dbContext;
        _storageRepository = new Lazy<IStorageRepository>(() => new StorageRepository(dbContext));
        _fileUploadSummaryRepository = new Lazy<IFileUploadSummaryRepository>(() => new FileUploadSummaryRepository(dbContext));
    }

    public IStorageRepository Storage
    {
        get { return _storageRepository.Value; }
    }

    public IFileUploadSummaryRepository FileUploadSummary
    {
        get { return _fileUploadSummaryRepository.Value; }
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}

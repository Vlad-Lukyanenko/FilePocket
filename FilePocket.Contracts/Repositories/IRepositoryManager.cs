namespace FilePocket.Contracts.Repositories;

public interface IRepositoryManager
{
    IStorageRepository Storage { get; }
    
    IFolderRepository Folder { get; }

    IFileUploadSummaryRepository FileUploadSummary { get; }

    Task SaveChangesAsync();
}

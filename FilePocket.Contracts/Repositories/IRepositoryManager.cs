namespace FilePocket.Contracts.Repositories;

public interface IRepositoryManager
{
    IStorageRepository Storage { get; }

    IFileUploadSummaryRepository FileUploadSummary { get; }

    Task SaveChangesAsync();
}

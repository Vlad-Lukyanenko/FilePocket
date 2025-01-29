namespace FilePocket.Contracts.Repositories;

public interface IRepositoryManager
{
    IPocketRepository Pocket { get; }

    ISharedFileRepository SharedFile { get; }
    
    IFolderRepository Folder { get; }

    IFileMetadataRepository FileMetadata { get; }

    Task SaveChangesAsync();
}

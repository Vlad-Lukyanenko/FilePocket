namespace FilePocket.Contracts.Services;

public interface IServiceManager
{
    IPocketService StorageService { get; }

    IFileService FileService { get; }

    IFolderService FolderService { get; }

    IAuthenticationService AuthenticationService { get; }
}

namespace FilePocket.Contracts.Services;

public interface IServiceManager
{
    IStorageService StorageService { get; }

    IFileService FileService { get; }

    IAuthenticationService AuthenticationService { get; }
}

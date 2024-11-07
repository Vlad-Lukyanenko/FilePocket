namespace FilePocket.Contracts.Services;

public interface IServiceManager
{
    IPocketService StorageService { get; }

    IFileService FileService { get; }

    IAuthenticationService AuthenticationService { get; }
}

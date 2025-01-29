using AutoMapper;
using FilePocket.Contracts.Repositories;
using FilePocket.Contracts.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FilePocket.Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IPocketService> _pocketService;
    private readonly Lazy<ISharedFileService> _sharedFileService;
    private readonly Lazy<IFileService> _fileService;
    private readonly Lazy<IFolderService> _folderService;
    private readonly Lazy<IAuthenticationService> _authenticationService;


    public ServiceManager(
        IRepositoryManager repositoryManager,
        IMapper mapper,
        ILoggerService logger,
        IConfiguration configuration,
        UserManager<User> userManager,
        IOptions<JwtConfigurationModel> options,
        IUploadService uploadService,
        IImageService imageService)
    {
        _pocketService = new Lazy<IPocketService>(() => new PocketService(repositoryManager, mapper, configuration));
        _sharedFileService = new Lazy<ISharedFileService>(() => new SharedFileService(repositoryManager, mapper));
        _fileService = new Lazy<IFileService>(() => new FileService(repositoryManager, configuration, uploadService, imageService, mapper));
        _folderService = new Lazy<IFolderService>(() => new FolderService(repositoryManager, mapper));
        _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, userManager, options, mapper));
    }

    public IPocketService PocketService => _pocketService.Value;

    public ISharedFileService SharedFileService => _sharedFileService.Value;

    public IFileService FileService => _fileService.Value;

    public IFolderService FolderService => _folderService.Value;

    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}

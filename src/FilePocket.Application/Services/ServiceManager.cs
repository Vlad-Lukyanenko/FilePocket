using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models.Configuration;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FilePocket.Application.Services;

public class ServiceManager(
    IRepositoryManager repositoryManager,
    IMapper mapper,
    ILoggerService logger,
    IConfiguration configuration,
    UserManager<User> userManager,
    IOptions<JwtConfigurationModel> options,
    IOptions<AccountConsumptionConfigurationModel> consumptionOptions,
    IImageService imageService)
    : IServiceManager
{
    private readonly Lazy<IPocketService> _pocketService = new(() => new PocketService(repositoryManager, mapper, configuration));
    private readonly Lazy<ISharedFileService> _sharedFileService = new(() => new SharedFileService(repositoryManager, mapper));
    private readonly Lazy<IFileService> _fileService = new(() => new FileService(repositoryManager, configuration, imageService, mapper));
    private readonly Lazy<IFolderService> _folderService = new(() => new FolderService(repositoryManager, mapper));
    private readonly Lazy<IAuthenticationService> _authenticationService = new(() => new AuthenticationService(logger, userManager, options, consumptionOptions, mapper));
    private readonly Lazy<IBookmarkService> _bookmarkService = new(() => new BookmarkService(repositoryManager, mapper));
    private readonly Lazy<IProfileService> _profileService = new(() => new ProfileService(repositoryManager, mapper));

    public IPocketService PocketService => _pocketService.Value;

    public ISharedFileService SharedFileService => _sharedFileService.Value;

    public IFileService FileService => _fileService.Value;

    public IFolderService FolderService => _folderService.Value;

    public IAuthenticationService AuthenticationService => _authenticationService.Value;

    public IBookmarkService BookmarkService => _bookmarkService.Value;

    public IProfileService ProfileService => _profileService.Value;
}

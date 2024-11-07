using AutoMapper;
using FilePocket.Contracts.Repositories;
using FilePocket.Contracts.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FilePocket.Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IPocketService> _storageService;
    private readonly Lazy<IFileService> _fileService;
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
        _storageService = new Lazy<IPocketService>(() => new StorageService(repositoryManager, mapper, configuration));
        _fileService = new Lazy<IFileService>(() => new FileService(repositoryManager, configuration, uploadService, imageService, mapper));
        _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(mapper, logger, userManager, options));
    }

    public IPocketService StorageService => _storageService.Value;

    public IFileService FileService => _fileService.Value;

    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}

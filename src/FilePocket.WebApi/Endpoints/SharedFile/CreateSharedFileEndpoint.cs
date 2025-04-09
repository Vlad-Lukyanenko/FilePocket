using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.SharedFile;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.SharedFile;

public class CreateSharedFileEndpoint : BaseEndpointWithoutResponse<SharedFileRequest>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public CreateSharedFileEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post("api/files/shared");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(SharedFileRequest file, CancellationToken cancellationToken)
    {
        var fileToCreate = _mapper.Map<SharedFileModel>(file);

        await _service.SharedFileService.CreateAsync(UserId, fileToCreate);

        await SendNoContentAsync();
    }
}

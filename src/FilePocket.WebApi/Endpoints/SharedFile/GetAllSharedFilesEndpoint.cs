using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.SharedFile;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.SharedFile;

public class GetAllSharedFilesEndpoint : BaseEndpointWithoutRequest<List<SharedFileViewResponse>>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public GetAllSharedFilesEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("api/files/shared");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var sharedFiles = _service.SharedFileService.GetAllAsync(UserId, trackChanges: false);

        var response = _mapper.Map<List<SharedFileViewResponse>>(sharedFiles);

        await SendOkAsync(response, cancellationToken);
    }
}

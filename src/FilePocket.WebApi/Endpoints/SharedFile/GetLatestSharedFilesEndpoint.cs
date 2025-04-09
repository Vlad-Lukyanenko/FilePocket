using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.SharedFile;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.SharedFile;

public class GetLatestSharedFilesEndpoint : BaseEndpointWithoutRequest<List<SharedFileViewResponse>>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public GetLatestSharedFilesEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("api/files/shared/latest");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var latestSharedFiles = await _service.SharedFileService.GetLatestAsync(UserId, 10);

        var response = _mapper.Map<List<SharedFileViewResponse>>(latestSharedFiles);

        await SendOkAsync(response, cancellationToken);
    }
}

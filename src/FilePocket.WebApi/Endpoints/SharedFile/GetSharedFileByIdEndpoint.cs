using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.SharedFile;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.SharedFile;

public class GetSharedFileByIdEndpoint : BaseEndpointWithoutRequest<SharedFileResponse>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public GetSharedFileByIdEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("api/files/shared/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");
        var sharedFile = await _service.SharedFileService.GetByIdAsync(id);

        var response = _mapper.Map<SharedFileResponse>(sharedFile!);

        await SendOkAsync(response, cancellationToken);
    }
}

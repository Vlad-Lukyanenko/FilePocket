using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Pocket;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;
using IMapper = MapsterMapper.IMapper;

namespace FilePocket.WebApi.Endpoints.Pocket;

public class GetAllCustomPocketsEndpoint : BaseEndpointWithoutRequest<List<GetPocketResponse>>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public GetAllCustomPocketsEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("api/pockets/all");
        PreProcessor<AuthorizationProcessor<EmptyRequest>>();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var pockets = await _service.PocketService.GetAllCustomByUserIdAsync(UserId, trackChanges: false);

        var response = _mapper.Map<List<GetPocketResponse>>(pockets);

        await SendOkAsync(response, cancellationToken);
    }
}

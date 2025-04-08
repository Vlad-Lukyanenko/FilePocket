using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Pocket;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;
using IMapper = MapsterMapper.IMapper;

namespace FilePocket.WebApi.Endpoints.Pocket;

public class GetPocketByIdEndpoint : BaseEndpointWithoutRequest<GetPocketResponse>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public GetPocketByIdEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("api/pockets/{pocketId:guid}");
        PreProcessor<AuthorizationProcessor<EmptyRequest>>();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var pocketId = Route<Guid>("pocketId");
        var pocket = await _service.PocketService.GetByIdAsync(UserId, pocketId, trackChanges: false);

        var response = _mapper.Map<GetPocketResponse>(pocket);

        await SendOkAsync(response, cancellationToken);
    }
}

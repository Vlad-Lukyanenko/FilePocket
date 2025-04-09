using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Pocket;
using FilePocket.Domain.Models;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Pocket;

public class UpdatePocketEndpoint : BaseEndpointWithoutResponse<CreateAndUpdatePocketRequest>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public UpdatePocketEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Put("api/pockets/{id:guid}");
        PreProcessor<AuthorizationProcessor<CreateAndUpdatePocketRequest>>();
    }

    public override async Task HandleAsync(CreateAndUpdatePocketRequest pocket, CancellationToken cancellationToken)
    {
        var pocketId = Route<Guid>("id");
        var pocketToUpdate = _mapper.Map<PocketForManipulationsModel>(pocket);

        await _service.PocketService.UpdatePocketAsync(pocketId, pocketToUpdate, trackChanges: true);

        await SendNoContentAsync();
    }
}

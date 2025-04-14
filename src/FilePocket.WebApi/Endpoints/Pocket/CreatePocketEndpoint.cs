using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Pocket;
using FilePocket.Domain.Models;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;
using IMapper = MapsterMapper.IMapper;

namespace FilePocket.WebApi.Endpoints.Pocket;

public class CreatePocketEndpoint : BaseEndpoint<CreateAndUpdatePocketRequest, GetPocketResponse>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public CreatePocketEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post("api/pockets");
        PreProcessor<AuthorizationProcessor<CreateAndUpdatePocketRequest>>();
    }

    public override async Task HandleAsync(CreateAndUpdatePocketRequest pocket, CancellationToken cancellationToken)
    {
        var pocketToCreate = _mapper.Map<PocketForManipulationsModel>(pocket);

        var createdPocket = await _service.PocketService.CreatePocketAsync(pocketToCreate);
        var response = _mapper.Map<GetPocketResponse>(createdPocket);

        await SendOkAsync(response, cancellationToken);
    }
}

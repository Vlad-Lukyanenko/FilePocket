using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Trash;

public class MovePocketToTrashEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly IServiceManager _service;

    public MovePocketToTrashEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Put("api/trash/pockets/{pocketId:guid}");
        PreProcessor<AuthorizationProcessor<EmptyRequest>>();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var pocketId = Route<Guid>("pocketId");
        await _service.PocketService.MoveToTrash(UserId, pocketId);

        await SendNoContentAsync();
    }
}

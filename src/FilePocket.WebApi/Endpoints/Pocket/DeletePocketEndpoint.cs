using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Pocket;

public class DeletePocketEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly IServiceManager _service;

    public DeletePocketEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Delete("api/pockets/{id:guid}");
        PreProcessor<AuthorizationProcessor<EmptyRequest>>();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");
        await _service.FolderService.DeleteByPocketIdAsync(id);
        await _service.PocketService.DeletePocketAsync(UserId, id, trackChanges: false);

        await SendNoContentAsync();
    }
}

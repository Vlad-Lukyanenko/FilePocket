using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;
using Hangfire;

namespace FilePocket.WebApi.Endpoints.Trash;

public class ClearAllTrashEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly ITrashService _trashService;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public ClearAllTrashEndpoint(ITrashService trashService, IBackgroundJobClient backgroundJobClient)
    {
        _trashService = trashService;
        _backgroundJobClient = backgroundJobClient;
    }

    public override void Configure()
    {
        Delete("api/trash/clearall");
        PreProcessor<AuthorizationProcessor<EmptyRequest>>();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        _backgroundJobClient.Enqueue(() => _trashService.ClearAllTrashAsync(UserId));

        await SendNoContentAsync();
    }
}

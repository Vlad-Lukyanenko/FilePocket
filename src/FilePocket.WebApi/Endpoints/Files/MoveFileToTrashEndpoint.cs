using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Files;

public class MoveFileToTrashEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly IServiceManager _service;

    public MoveFileToTrashEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Put("api/files/{fileId:guid}");
        PreProcessor<AuthorizationProcessor<EmptyRequest>>();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var fileId = Route<Guid>("fileId");
        await _service.FileService.MoveToTrash(UserId, fileId);

        await SendNoContentAsync();
    }
}

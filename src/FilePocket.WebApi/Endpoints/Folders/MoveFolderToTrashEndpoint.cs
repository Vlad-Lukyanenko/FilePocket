using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Folders;

public class MoveFolderToTrashEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly IServiceManager _service;

    public MoveFolderToTrashEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Put("api/folders/{id:guid}");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");
        await _service.FolderService.MoveToTrashAsync(id);

        await SendOkAsync(cancellationToken);
    }
}

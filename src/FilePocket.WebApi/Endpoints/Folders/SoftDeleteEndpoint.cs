using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Folders;

public class SoftDeleteEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly IServiceManager _service;

    public SoftDeleteEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Delete("api/folders/soft/{id:guid}");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");
        await _service.FolderService.MoveToTrashAsync(id);

        await SendOkAsync(cancellationToken);
    }
}

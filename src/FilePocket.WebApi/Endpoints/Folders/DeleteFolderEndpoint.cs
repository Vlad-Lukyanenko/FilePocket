using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using Hangfire;

namespace FilePocket.WebApi.Endpoints.Folders;

public class DeleteFolderEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly IServiceManager _service;

    public DeleteFolderEndpoint(IServiceManager service, IBackgroundJobClient backgroundJobClient)
    {
        _service = service;
    }

    public override void Configure()
    {
        Delete("api/folders/{folderId:guid}");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        await _service.FolderService.DeleteAsync(FolderId!.Value);

        await SendNoContentAsync();
    }
}

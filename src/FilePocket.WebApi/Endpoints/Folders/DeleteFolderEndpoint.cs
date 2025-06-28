using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Folders;

public class DeleteFolderEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly IServiceManager _service;

    public DeleteFolderEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Delete("api/folders/{folderId:guid}");
    }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
        
        //TODO: Implement files deletion on folder deletion
        await _service.FolderService.DeleteAsync(FolderId!.Value);

        await SendOkAsync(cancellationToken);
    }
}

using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Folders;

public class DeleteEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly IServiceManager _service;

    public DeleteEndpoint(IServiceManager service)
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

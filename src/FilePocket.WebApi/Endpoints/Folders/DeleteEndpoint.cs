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
            var folder = await _service.FolderService.GetAsync(FolderId!.Value);

            if (folder?.FolderType == Domain.Enums.FolderType.Documents)
            {
                try
                {
                    await _service.NoteService.BulkDeleteAsync(FolderId!.Value, cancellationToken);
                }
                catch
                {
                    await SendErrorsAsync(cancellation: cancellationToken);
                }
            }

            await _service.FolderService.DeleteAsync(FolderId!.Value);

        await SendOkAsync(cancellationToken);
    }
}

using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Trash;

public class MoveFileToTrashEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly IFileService _fileService;

    public MoveFileToTrashEndpoint(IFileService fileService)
    {
        _fileService = fileService;
    }

    public override void Configure()
    {
        Put("api/trash/files/{fileId:guid}");
        PreProcessor<AuthorizationProcessor<EmptyRequest>>();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var fileId = Route<Guid>("fileId");
        await _fileService.MoveToTrash(UserId, fileId);

        await SendNoContentAsync();
    }
}

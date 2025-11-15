using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Files;

public class MoveFileToTrashEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly IFileService _minioFileService;

    public MoveFileToTrashEndpoint(IFileService minioFileService)
    {
        _minioFileService = minioFileService;
    }

    public override void Configure()
    {
        Put("api/files/{fileId:guid}");
        PreProcessor<AuthorizationProcessor<EmptyRequest>>();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var fileId = Route<Guid>("fileId");
        await _minioFileService.MoveToTrash(UserId, fileId);

        await SendNoContentAsync();
    }
}

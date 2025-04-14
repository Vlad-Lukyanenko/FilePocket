using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.SharedFile;

public class DownloadSharedFileByIdEndpoint : BaseEndpointWithoutRequest<byte[]>
{
    private readonly IServiceManager _service;

    public DownloadSharedFileByIdEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Get("api/files/shared/download/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");
        var sharedFile = await _service.SharedFileService.DownloadFileAsync(id);

        await SendOkAsync(sharedFile!, cancellationToken);
    }
}

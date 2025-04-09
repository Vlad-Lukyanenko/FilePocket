using FastEndpoints;
using FilePocket.Application.Interfaces.Services;

namespace FilePocket.WebApi.Endpoints.SharedFile;

public class DeleteSharedFileEndpoint : EndpointWithoutRequest
{
    private readonly IServiceManager _service;

    public DeleteSharedFileEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Delete("api/files/shared/{id:guid}");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");
        await _service.SharedFileService.Delete(id);

        await SendNoContentAsync();
    }
}

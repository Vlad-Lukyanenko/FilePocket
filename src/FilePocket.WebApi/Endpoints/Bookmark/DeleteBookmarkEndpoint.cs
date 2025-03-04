using FastEndpoints;
using FilePocket.Application.Interfaces.Services;

namespace FilePocket.WebApi.Endpoints.Bookmark;

public class DeleteBookmarkEndpoint : EndpointWithoutRequest
{
    private readonly IServiceManager _service;

    public DeleteBookmarkEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Delete("api/bookmark/{id:guid}");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");
        await _service.BookmarkService.DeleteBookmarkAsync(id);

        await SendNoContentAsync();
    }
}

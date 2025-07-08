using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Bookmark;

public class MoveBookmarkToTrashEndpoint : BaseEndpointWithoutRequestAndResponse
{
    private readonly IServiceManager _service;

    public MoveBookmarkToTrashEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Delete("api/bookmark/soft/{id:guid}");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var id = Route<Guid>("id");
        await _service.BookmarkService.MoveToTrashAsync(id);

        await SendOkAsync(cancellationToken);
    }
}

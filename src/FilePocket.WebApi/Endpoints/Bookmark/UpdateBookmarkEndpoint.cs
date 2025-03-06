using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Bookmark;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Bookmark;

public class UpdateBookmarkEndpoint : BaseEndpointWithoutResponse<UpdateBookmarkRequest>
{
    private readonly IServiceManager _service;

    public UpdateBookmarkEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Put("api/bookmark");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(UpdateBookmarkRequest bookmark, CancellationToken cancellationToken)
    {
        await _service.BookmarkService.UpdateBookmarkAsync(bookmark);

        await SendNoContentAsync();
    }
}

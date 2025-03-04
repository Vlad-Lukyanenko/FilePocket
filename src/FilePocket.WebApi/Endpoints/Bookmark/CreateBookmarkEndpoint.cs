using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Bookmark;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Bookmark;

public class CreateBookmarkEndpoint : BaseEndpoint<BookmarkModel, BookmarkCreatedResponse>
{
    private readonly IServiceManager _service;

    public CreateBookmarkEndpoint(IServiceManager service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Post("api/bookmark");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(BookmarkModel bookmark, CancellationToken cancellationToken)
    {
        bookmark.UserId = UserId;

        var createdBookmark = await _service.BookmarkService.CreateBookmarkAsync(bookmark);
        var response = new BookmarkCreatedResponse() { Id = createdBookmark.Id, Title = createdBookmark.Title, Url = createdBookmark.Url };

        await SendOkAsync(response, cancellationToken);
    }
}

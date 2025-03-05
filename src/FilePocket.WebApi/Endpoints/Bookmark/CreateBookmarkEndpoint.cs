using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Bookmark;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Bookmark;

public class CreateBookmarkEndpoint : BaseEndpoint<CreateBookmarkRequest, CreateBookmarkResponse>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public CreateBookmarkEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post("api/bookmark");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CreateBookmarkRequest bookmark, CancellationToken cancellationToken)
    {
        var bookmarkToCreate = _mapper.Map<BookmarkModel>(bookmark);
        bookmarkToCreate.UserId = UserId;

        var createdBookmark = await _service.BookmarkService.CreateBookmarkAsync(bookmarkToCreate);
        var response = new CreateBookmarkResponse() { Id = createdBookmark.Id, Title = createdBookmark.Title, Url = createdBookmark.Url };

        await SendOkAsync(response, cancellationToken);
    }
}

using AutoMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Bookmark;

public class UpdateBookmarkEndpoint : BaseEndpointWithoutResponse<BookmarkModel>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public UpdateBookmarkEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Put("api/bookmark");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(BookmarkModel bookmark, CancellationToken cancellationToken)
    {
        await _service.BookmarkService.UpdateBookmarkAsync(bookmark);

        await SendNoContentAsync();
    }
}

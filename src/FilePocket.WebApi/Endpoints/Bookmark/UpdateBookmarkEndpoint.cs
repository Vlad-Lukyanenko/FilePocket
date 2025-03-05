using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Bookmark;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Bookmark;

public class UpdateBookmarkEndpoint : BaseEndpointWithoutResponse<UpdateBookmarkRequest>
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

    public override async Task HandleAsync(UpdateBookmarkRequest bookmark, CancellationToken cancellationToken)
    {
        var bookmarkToUpdate = _mapper.Map<BookmarkModel>(bookmark);

        await _service.BookmarkService.UpdateBookmarkAsync(bookmarkToUpdate);

        await SendNoContentAsync();
    }
}

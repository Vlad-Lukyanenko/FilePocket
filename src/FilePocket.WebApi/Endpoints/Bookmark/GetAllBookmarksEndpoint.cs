using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Bookmark;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Bookmark;

public class GetAllBookmarksEndpoint : BaseEndpointWithoutRequest<List<GetBookmarksResponse>>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public GetAllBookmarksEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("api/bookmark/all/{isSoftDeleted:bool}");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var isSoftDeleted = Route<bool>("isSoftDeleted");

        var bookmarks = _service.BookmarkService.GetAll(UserId, isSoftDeleted, trackChanges: false);

        var response = _mapper.Map<List<GetBookmarksResponse>>(bookmarks);

        await SendOkAsync(response, cancellationToken);
    }
}

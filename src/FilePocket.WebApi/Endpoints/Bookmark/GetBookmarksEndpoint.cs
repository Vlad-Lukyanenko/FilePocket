using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Bookmark;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Bookmark;

public class GetBookmarksEndpoint : BaseEndpointWithoutRequest<List<GetBookmarksResponse>>
{
    private readonly IServiceManager _service;
    private readonly IMapper _mapper;

    public GetBookmarksEndpoint(IServiceManager service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("api/pockets/{pocketId:guid}/{isSoftDeleted:bool}/bookmarks",
            "api/pockets/{pocketId:guid}/folders/{folderId:guid?}/{isSoftDeleted:bool}/bookmarks");

        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var pocketId = Route<Guid>("pocketId");
        var folderId = Route<Guid?>("folderId", false);
        var isSoftDeleted = Route<bool>("isSoftDeleted");

        var bookmarks = await _service.BookmarkService.GetAllAsync(UserId, pocketId, folderId, isSoftDeleted, trackChanges: false);

        var response = _mapper.Map<List<GetBookmarksResponse>>(bookmarks);

        await SendOkAsync(response, cancellationToken);
    }
}

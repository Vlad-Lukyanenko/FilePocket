using AutoMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Bookmark;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Bookmark;

public class GetAllBookmarksEndpoint : BaseEndpointWithoutRequest<List<GetAllBookmarksResponse>>
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
        Get("api/bookmark/all");
        AuthSchemes("Bearer");
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var bookmrks = _service.BookmarkService.GetAll(UserId, trackChanges: false);

        var response = _mapper.Map<List<GetAllBookmarksResponse>>(bookmrks);

        await SendOkAsync(response, cancellationToken);
    }
}

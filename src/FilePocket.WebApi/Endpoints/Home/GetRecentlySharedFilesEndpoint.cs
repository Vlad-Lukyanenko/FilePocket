using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Home;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Home
{
    public class GetRecentlySharedFilesEndpoint : BaseEndpointWithoutRequest<List<GetRecentlySharedFilesResponse>>
    {
        private readonly IServiceManager _service;
        private readonly IMapper _mapper;

        public GetRecentlySharedFilesEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Get("api/home/files/shared/recent");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var recentFiles = await _service.SharedFileService.GetLatestAsync(UserId, 10);

            var response = _mapper.Map<List<GetRecentlySharedFilesResponse>>(recentFiles);

            await SendOkAsync(response, cancellationToken);
        }
    }
}

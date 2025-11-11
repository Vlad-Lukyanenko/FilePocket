using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Home;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Home
{
    public class GetRecentlySharedFilesEndpoint : BaseEndpointWithoutRequest<List<GetRecentlySharedFilesResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IFileService _minioFileService;


        public GetRecentlySharedFilesEndpoint(IMapper mapper, IFileService minioFileService)
        {
            _mapper = mapper;
            _minioFileService = minioFileService;
        }

        public override void Configure()
        {
            Get("api/home/files/shared/recent");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var recentFiles = await _minioFileService.GetLatestAsync(UserId, 10);

            var response = _mapper.Map<List<GetRecentlySharedFilesResponse>>(recentFiles);

            await SendOkAsync(response, cancellationToken);
        }
    }
}

using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Home;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Home
{
    public class GetRecentlyUploadedFilesEndpoint : BaseEndpointWithoutRequest<List<GetRecentlyUploadedFilesResponse>>
    {
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public GetRecentlyUploadedFilesEndpoint(IFileService fileService, IMapper mapper)
        {
            _fileService = fileService;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Get("api/home/files/recent");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var recentFiles = await _fileService.GetLatestAsync(UserId, 10);

            var response = _mapper.Map<List<GetRecentlyUploadedFilesResponse>>(recentFiles);

            await SendOkAsync(response, cancellationToken);
        }
    }
}

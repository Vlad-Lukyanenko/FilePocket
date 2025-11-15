using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Home;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Home
{
    public class GetRecentlyUploadedFilesEndpoint : BaseEndpointWithoutRequest<List<GetRecentlyUploadedFilesResponse>>
    {
        private readonly IServiceManager _service;
        private readonly IMapper _mapper;

        public GetRecentlyUploadedFilesEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Get("api/home/files/recent");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var recentFiles = await _service.FileService.GetLatestAsync(UserId, 10);

            var response = _mapper.Map<List<GetRecentlyUploadedFilesResponse>>(recentFiles);

            await SendOkAsync(response, cancellationToken);
        }
    }
}

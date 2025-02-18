using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Home;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Home
{
    public class GetRecentlySharedFilesEndpoint : BaseEndpointWithoutRequest<List<GetRecentlySharedFilesResponse>>
    {
        private readonly IServiceManager _service;

        public GetRecentlySharedFilesEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Get("api/home/recent-files/shared");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var recentFiles = await _service.SharedFileService.GetLatestAsync(UserId, 10);

            var response = new List<GetRecentlySharedFilesResponse>();

            foreach (var f in recentFiles)
            {
                response.Add(new GetRecentlySharedFilesResponse()
                {
                    SharedFileId = f.SharedFileId,
                    FileType = f.FileType!.Value,
                    OriginalName = f.OriginalName!
                });
            }

            await SendOkAsync(response, cancellationToken);
        }
    }
}

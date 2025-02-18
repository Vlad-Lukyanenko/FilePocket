using FilePocket.Application.Interfaces.Services;
using FilePocket.Contracts.Home;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Home
{
    public class GetRecentlyUploadedFilesEndpoint : BaseEndpointWithoutRequest<List<GetRecentlyUploadedFilesResponse>>
    {
        private readonly IServiceManager _service;

        public GetRecentlyUploadedFilesEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Get("api/home/recent-files");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var recentFiles = await _service.FileService.GetRecentFiles(UserId, 10);

            var response = new List<GetRecentlyUploadedFilesResponse>();

            foreach (var f in recentFiles)
            {
                response.Add(new GetRecentlyUploadedFilesResponse()
                {
                    Id = f.Id,
                    PocketId = f.PocketId!.Value,
                    FolderId = f.FolderId,
                    FileType = f.FileType!.Value,
                    OriginalName = f.OriginalName!
                });
            }

            await SendOkAsync(response, cancellationToken);
        }
    }
}

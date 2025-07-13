using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Files
{
    public class GetImageThumbnailsEndpoint : BaseEndpoint<GetImageThumbnailsRequest, IEnumerable<FileResponseModel>>
    {
        private readonly IServiceManager _service;
        public GetImageThumbnailsEndpoint(IServiceManager service)
        {
            _service = service;
        }
        public override void Configure()
        {
            Post("api/files/thumbnails/{size:int}");
            AuthSchemes("Bearer");
        }
        public override async Task HandleAsync(GetImageThumbnailsRequest request, CancellationToken cancellationToken)
        {
            var thumbnails = await _service.FileService.GetThumbnailsAsync(UserId, request.ImageIds, request.Size);

            if (thumbnails == null || !thumbnails.Any())
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(thumbnails, cancellationToken);
        }
    }

    public class GetImageThumbnailsRequest
    {
        public Guid[] ImageIds { get; set; } = [];

        [BindFrom("size")]
        public int Size { get; set; }
    }
}


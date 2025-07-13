using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Files
{
    public class GetImageThumbnailEndpoint : BaseEndpointWithoutRequest<FileResponseModel>
    {
        private readonly IServiceManager _service;

        public GetImageThumbnailEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Get("api/files/{imageId:guid}/thumbnail/{size}");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var imageId = Route<Guid>("imageId");
            var size = Route<int>("size");

            var thumbnail = await _service.FileService.GetThumbnailAsync(UserId, imageId, size);

            if (thumbnail == null)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(thumbnail, cancellationToken);
        }
    }
}

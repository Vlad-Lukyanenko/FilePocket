using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Files
{
    public class DeleteFileEndpoint : BaseEndpointWithoutRequestAndResponse
    {
        private readonly IServiceManager _service;
        private readonly IMinioService _minioService;


        public DeleteFileEndpoint(IServiceManager service, IMinioService minioService)
        {
            _service = service;
            _minioService = minioService;
        }

        public override void Configure()
        {
            Delete("api/files/{fileId:guid}");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var fileId = Route<Guid>("fileId");

            try
            {
                await _minioService.RemoveFileAsync(UserId, fileId, cancellationToken);
                await _service.FileService.RemoveFileAsync(UserId, fileId, cancellationToken);

                await SendNoContentAsync(cancellationToken);
            }
            catch
            {
                await SendNotFoundAsync(cancellationToken);

            }
        }
    }
}

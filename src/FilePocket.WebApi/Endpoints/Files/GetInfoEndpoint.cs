using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;


namespace FilePocket.WebApi.Endpoints.Files
{
    public class GetInfoEndpoint : BaseEndpointWithoutRequest<FileResponseModel>
    {
        private readonly IServiceManager _service;
        private readonly IFileService _minioFileService;

        public GetInfoEndpoint(IServiceManager service, IFileService minioFileService)
        {
            _service = service;
            _minioFileService = minioFileService;
        }

        public override void Configure()
        {
            Get("api/files/{fileId:guid}/info");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var fileId = Route<Guid>("fileId");

            var file = await _minioFileService.GetFileByUserIdAndIdAsync(UserId, fileId);

            if (file == null)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(file, cancellationToken);
        }
    }


}

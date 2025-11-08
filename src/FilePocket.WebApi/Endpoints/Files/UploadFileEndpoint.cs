using FilePocket.Application.Interfaces.Services;
using FilePocket.Application.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Files
{
    public class UploadFileEndpoint : BaseEndpoint<FileInformationModel, FileResponseModel>
    {
        private readonly IServiceManager _service;
        private readonly IFileService _minioFileService;
        public UploadFileEndpoint(IServiceManager service, IFileService minioFileService)
        {
            _service = service;
            _minioFileService = minioFileService;
        }

        public override void Configure()
        {
            Post("api/files");
            AllowFormData();
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(FileInformationModel request, CancellationToken cancellationToken)
        {
            if (request.FolderId == Guid.Empty) // temporary for compatibility
            {
                request.FolderId = null;
            }

            try
            {
                var minioFMetadata = await _minioFileService.UploadFileAsync(
                    UserId,
                    request.File!,
                    request.PocketId,
                    request.FolderId,
                    cancellationToken);

                await SendOkAsync(minioFMetadata!, cancellationToken);
            }
            catch
            {
                await SendErrorsAsync(cancellation: cancellationToken);
            }
        }


    }
}

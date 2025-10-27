using FilePocket.Application.Interfaces.Services;
using FilePocket.Application.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Files
{
    public class UploadFileEndpoint : BaseEndpoint<FileInformationModel, FileResponseModel>
    {
        private readonly IServiceManager _service;
        private readonly IMinioService _minioService;
        public UploadFileEndpoint(IServiceManager service, IMinioService minioService)
        {
            _service = service;
            _minioService = minioService;
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
                var fileMetadata = await _service.FileService.UploadFileAsync(
                    UserId, 
                    request.File!, 
                    request.PocketId, 
                    request.FolderId, 
                    cancellationToken);

                await _minioService.UploadFileAsync(
                    request.File!, 
                    UserId, 
                    request.PocketId, 
                    fileMetadata!.FileType!.Value, 
                    fileMetadata.Id, 
                    cancellationToken);

                await SendOkAsync(fileMetadata!, cancellationToken);
            }
            catch
            {
                await SendErrorsAsync(cancellation: cancellationToken);
            }
        }


    }
}

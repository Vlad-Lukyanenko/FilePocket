using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Files
{
    public class UploadFileEndpoint : BaseEndpoint<FileInformationModel, FileResponseModel>
    {
        private readonly IServiceManager _service;
        public UploadFileEndpoint(IServiceManager service)
        {
            _service = service;
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
            {
                request.FolderId = null;
            }

            try
            {
                var fileMetadata = await _service.FileService.UploadFileAsync(
                    UserId, request.File!, request.PocketId, request.FolderId, cancellationToken);

                await SendOkAsync(fileMetadata!, cancellationToken);
            }
            catch
            {
                await SendErrorsAsync(cancellation: cancellationToken);
            }
        }


    }
}

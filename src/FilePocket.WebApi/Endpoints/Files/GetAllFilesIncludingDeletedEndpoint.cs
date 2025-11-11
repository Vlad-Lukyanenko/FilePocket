using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Files
{
    public class GetAllFilesIncludingDeletedEndpoint :BaseEndpointWithoutRequest<IEnumerable<FileResponseModel>>
    {
        private readonly IFileService _minioFileService;
        public GetAllFilesIncludingDeletedEndpoint(IFileService minioFileService)
        {
            _minioFileService = minioFileService;
        }
        public override void Configure()
        {
            Get("api/pockets/{pocketId:guid}/files");
            AuthSchemes("Bearer");
        }
        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var pocketId = Route<Guid>("pocketId");

            var files = await _minioFileService.GetAllFilesIncludingDeletedAsync(UserId, pocketId);

            if (files == null)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(files, cancellationToken);
        }
    }
}

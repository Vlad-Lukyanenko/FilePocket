using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Files
{
    public class GetAllFilesIncludingDeletedEndpoint :BaseEndpointWithoutRequest<IEnumerable<FileResponseModel>>
    {
        private readonly IServiceManager _service;
        public GetAllFilesIncludingDeletedEndpoint(IServiceManager service)
        {
            _service = service;
        }
        public override void Configure()
        {
            Get("api/pockets/{pocketId:guid}/files");
            AuthSchemes("Bearer");
        }
        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var pocketId = Route<Guid>("pocketId");

            var files = await _service.FileService.GetAllFilesIncludingDeletedAsync(UserId, pocketId);

            if (files == null)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(files, cancellationToken);
        }
    }
}

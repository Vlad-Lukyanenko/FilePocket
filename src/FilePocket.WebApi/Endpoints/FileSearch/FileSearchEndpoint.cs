using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using static System.Net.WebRequestMethods;

namespace FilePocket.WebApi.Endpoints.FileSearch
{
    public class FileSearchEndpoint : BaseEndpointWithoutRequest<IEnumerable<FileSearchResponseModel>>
    {
        private readonly IServiceManager _service;

        public FileSearchEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/file-search/{partialName}");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var partialName = Route<string>("partialName");

            if (string.IsNullOrWhiteSpace(partialName))
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            var files = await _service.FileService.SearchEverywhereAsync(UserId, partialName);

            if (files == null || !files.Any())
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }
            await SendOkAsync(files, cancellationToken);
        }
    }
}

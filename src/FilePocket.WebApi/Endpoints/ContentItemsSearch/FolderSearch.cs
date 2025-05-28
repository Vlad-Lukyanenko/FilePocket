using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using static System.Net.WebRequestMethods;

namespace FilePocket.WebApi.Endpoints.ContentItemsSearch
{
    public class FolderSearch : BaseEndpointWithoutRequest<IEnumerable<FolderSearchResponseModel>>
    {
        private readonly IServiceManager _service;

        public FolderSearch(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/folder-search/{partialName}");
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

            var folders = await _service.FolderService.SearchAsync(UserId, partialName);

            if (folders == null || !folders.Any())
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(folders, cancellationToken);
        }
    }
}

using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using static System.Net.WebRequestMethods;

namespace FilePocket.WebApi.Endpoints.ContentItemsSearch
{
    public class SearchEndpoint : BaseEndpointWithoutRequest<IEnumerable<SearchResponseModel>>
    {
        private readonly IServiceManager _service;
        public SearchEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/search/{itemType}/{partialName}");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var partialName = Route<string>("partialName");
            var itemType = Route<string>("itemType");

            if (string.IsNullOrWhiteSpace(partialName))
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            if (string.IsNullOrWhiteSpace(itemType))
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            IEnumerable<SearchResponseModel> items = itemType.ToLower() switch
            {
                "file" => _service.FileService.SearchAsync(UserId, partialName).Result.Cast<FileSearchResponseModel>(),
                "bookmark" => _service.BookmarkService.SearchAsync(UserId, partialName).Result.Cast<BookmarkSearchResponseModel>(),
                "folder" => _service.FolderService.SearchAsync(UserId, partialName).Result.Cast<FolderSearchResponseModel>(),
                _ => throw new NotSupportedException($"Item type '{itemType}' is not supported.")
            };

            await SendOkAsync(items, cancellationToken);
        }
    }
}

using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using static System.Net.WebRequestMethods;


namespace FilePocket.WebApi.Endpoints.ContentItemsSearch
{
    public class BookmarkSearchEndpoint : BaseEndpointWithoutRequest<IEnumerable<BookmarkSearchResponseModel>> {

        private readonly IServiceManager _service;

        public BookmarkSearchEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/bookmark-search/{partialName}");
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
            var bookmarks = await _service.BookmarkService.SearchAsync(UserId, partialName);

            if (bookmarks == null || !bookmarks.Any())
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(bookmarks, cancellationToken);
        }
    }

}

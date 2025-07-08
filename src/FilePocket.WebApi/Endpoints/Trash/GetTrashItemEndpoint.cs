using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using static System.Net.WebRequestMethods;

namespace FilePocket.WebApi.Endpoints.Trash
{
    public class GetTrashItemEndpoint : BaseEndpointWithoutRequest<SearchResponseModel>
    {
        private readonly IServiceManager _service;

        public GetTrashItemEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/trash/{itemType}/{itemId}");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var itemType = Route<string>("itemType");
            var itemId = Route<Guid>("itemId");

            if (string.IsNullOrWhiteSpace(itemType) || itemId == Guid.Empty)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            SearchResponseModel? item = itemType.ToLower() switch
            {
                "file" => await _service.FileService.GetSoftDeletedAsync(itemId),
                "bookmark" => await _service.BookmarkService.GetSoftDeletedAsync(itemId),
                "folder" => await _service.FolderService.GetSoftDeletedAsync(itemId),
                _ => throw new NotSupportedException($"Item type '{itemType}' is not supported.")
            };

            if (item == null)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(item, cancellationToken);
        }
    }
    
}

using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using static System.Net.WebRequestMethods;

namespace FilePocket.WebApi.Endpoints.Trash
{
    public class GetAllTrashEndpoint : BaseEndpointWithoutRequest<IEnumerable<SearchResponseModel>>
    {
        private readonly IServiceManager _service;

        public GetAllTrashEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/trash/{itemType}");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var itemType = Route<string>("itemType");

            if (string.IsNullOrWhiteSpace(itemType))
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            IEnumerable<SearchResponseModel> items = itemType.ToLower() switch
            {
                "file" => _service.FileService.GetAllSoftDeletedAsync(UserId).Result.Cast<DeletedFileModel>(),
                "bookmark" => _service.BookmarkService.GetAllSoftDeletedAsync(UserId).Result.Cast<DeletedBookmarkModel>(),
                "folder" => _service.FolderService.GetAllSoftDeletedAsync(UserId).Result.Cast<DeletedFolderModel>(),
                _ => throw new NotSupportedException($"Item type '{itemType}' is not supported.")
            };

            await SendOkAsync(items, cancellationToken);
        }
    }
}

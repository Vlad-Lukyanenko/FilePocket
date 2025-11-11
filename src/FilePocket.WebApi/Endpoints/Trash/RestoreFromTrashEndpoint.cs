using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.EndpointProcessors;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Trash
{
    public class RestoreFromTrashEndpoint : BaseEndpointWithoutRequest<SearchResponseModel>
    {
        private readonly IServiceManager _service;
        private readonly IFileService _minioFileService;


        public RestoreFromTrashEndpoint(IServiceManager service, IFileService minioFileService)
        {
            _service = service;
            _minioFileService = minioFileService;
        }

        public override void Configure()
        {
            Put("api/trash/{itemType}/{itemId}/restore");
            PreProcessor<AuthorizationProcessor<EmptyRequest>>();
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

            switch (itemType)
            {
                case "file":
                    var folderId = await _minioFileService.RestoreFromTrashAsync(UserId, itemId);

                    if (folderId != null)
                    {
                        await _service.FolderService.RestoreParentFolderFromTrashAsync(folderId.Value);
                    }

                    break;
                case "bookmark":
                    await _service.BookmarkService.RestoreFromTrashAsync(itemId);
                    break;
                case "folder":
                    await _service.FolderService.RestoreFromTrashAsync(itemId);
                    break;
                default:
                    throw new NotSupportedException($"Item type '{itemType}' is not supported.");
            }

            await SendOkAsync(cancellationToken);
        }
    }
}

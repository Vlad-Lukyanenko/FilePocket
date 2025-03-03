using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class DeleteEndpoint : BaseEndpointWithoutRequestAndResponse
    {
        private readonly IServiceManager _service;

        public DeleteEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
        }

        public override void Configure()
        {
            Delete("api/folders/{folderId:guid}");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            await _service.FolderService.DeleteAsync(FolderId!.Value);

            await SendOkAsync(cancellationToken);
        }
    }
}

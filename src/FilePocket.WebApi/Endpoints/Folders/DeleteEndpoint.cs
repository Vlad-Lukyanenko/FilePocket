using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;
using Microsoft.AspNetCore.Routing;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class DeleteEndpoint : BaseEndpointWithoutRequestAndResponse
    {
        private readonly IServiceManager _service;
        private Guid FolderId => Guid.Parse(HttpContext.GetRouteValue("folderId").ToString() ?? Guid.Empty.ToString());

        public DeleteEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
        }

        public override void Configure()
        {
            Delete("folders/{folderId:guid}");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            await _service.FolderService.DeleteAsync(FolderId);

            await SendOkAsync(cancellationToken);
        }
    }
}

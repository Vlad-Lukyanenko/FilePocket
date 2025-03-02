using MapsterMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Routing;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class GetEndpoint : BaseEndpointWithoutRequest<FolderModel>
    {
        private readonly IServiceManager _service;
        private readonly IMapper _mapper;
        private Guid FolderId => Guid.Parse(HttpContext.GetRouteValue("folderId").ToString() ?? Guid.Empty.ToString());

        public GetEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Get("folders/{folderId:guid}");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var folder = await _service.FolderService.GetAsync(FolderId);

            var response = _mapper.Map<FolderModel>(folder);

            await SendOkAsync(response, cancellationToken);
        }
    }
}

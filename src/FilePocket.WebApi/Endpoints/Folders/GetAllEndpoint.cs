using MapsterMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using static System.Net.WebRequestMethods;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class GetAllEndpoint : BaseEndpointWithoutRequest<List<FolderModel>>
    {
        private readonly IServiceManager _service;
        private readonly IMapper _mapper;
        private Guid PocketId => Guid.Parse(HttpContext.GetRouteValue("pocketId").ToString() ?? Guid.Empty.ToString());

        public GetAllEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/pockets/{pocketId:guid}/folders", "api/folders");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var folders = await _service.FolderService.GetAllAsync(UserId, PocketId, null);

            var response = _mapper.Map<List<FolderModel>>(folders);

            await SendOkAsync(response, cancellationToken);
        }
    }
}

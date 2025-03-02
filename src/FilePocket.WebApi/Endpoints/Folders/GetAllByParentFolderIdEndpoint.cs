using MapsterMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using static System.Net.WebRequestMethods;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class GetAllByParentFolderIdEndpoint : BaseEndpointWithoutRequest<List<FolderModel>>
    {
        private readonly IServiceManager _service;
        private Guid PocketId => Guid.Parse(HttpContext.GetRouteValue("pocketId").ToString() ?? Guid.Empty.ToString());
        private Guid ParentFolderId => Guid.Parse(HttpContext.GetRouteValue("parentFolderId").ToString() ?? Guid.Empty.ToString());

        public GetAllByParentFolderIdEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("pockets/{pocketId:guid}/parent-folder/{parentFolderId:guid}/folders", "parent-folder/{parentFolderId:guid}/folders");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var folders = await _service.FolderService.GetAllAsync(UserId, PocketId, ParentFolderId);

            await SendOkAsync(folders, cancellationToken);
        }
    }
}

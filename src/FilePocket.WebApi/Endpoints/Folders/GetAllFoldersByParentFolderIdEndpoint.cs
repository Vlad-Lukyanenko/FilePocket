using MapsterMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using FilePocket.Domain.Models;
using static System.Net.WebRequestMethods;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class GetAllFoldersByParentFolderIdEndpoint : BaseEndpointWithoutRequest<List<FolderModel>>
    {
        private readonly IServiceManager _service;
      
        public GetAllFoldersByParentFolderIdEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/pockets/{pocketId:guid}/parent-folder/{parentFolderId:guid}/folders");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var folders = await _service.FolderService.GetAllAsync(UserId, PocketId, ParentFolderId);

            await SendOkAsync(folders, cancellationToken);
        }
    }
}

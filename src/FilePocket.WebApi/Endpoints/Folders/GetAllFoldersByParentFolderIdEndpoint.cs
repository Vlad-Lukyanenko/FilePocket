using MapsterMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using FilePocket.Domain.Models;
using static System.Net.WebRequestMethods;
using FilePocket.Contracts.Folders.Responses;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class GetAllFoldersByParentFolderIdEndpoint : BaseEndpointWithoutRequest<List<GetAllFoldersByParentFolderIdResponse>>
    {
        private readonly IServiceManager _service;
        private readonly IMapper _mapper;
      
        public GetAllFoldersByParentFolderIdEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/pockets/{pocketId:guid}/parent-folder/{parentFolderId:guid}/folders");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var folders = await _service.FolderService.GetAllAsync(UserId, PocketId, ParentFolderId);

            var response = new List<GetAllFoldersByParentFolderIdResponse>();
            foreach(var folder in folders)
            {
                response.Add(_mapper.Map<GetAllFoldersByParentFolderIdResponse>(folder));
            }

            await SendOkAsync(response, cancellationToken);
        }
    }
}

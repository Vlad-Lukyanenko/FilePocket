using MapsterMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using FilePocket.Domain.Models;
using static System.Net.WebRequestMethods;
using FilePocket.Contracts.Folders.Responses;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class GetAllFoldersEndpoint : BaseEndpointWithoutRequest<List<GetAllFoldersResponse>>
    {
        private readonly IServiceManager _service;
        private readonly IMapper _mapper;

        public GetAllFoldersEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/pockets/{pocketId:guid}/folders");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var folders = await _service.FolderService.GetAllAsync(UserId, PocketId, null);

            var response = new List<GetAllFoldersResponse>();
            foreach (var folder in folders)
            {
                response.Add(_mapper.Map<GetAllFoldersResponse>(folder));
            }

            await SendOkAsync(response, cancellationToken);
        }
    }
}

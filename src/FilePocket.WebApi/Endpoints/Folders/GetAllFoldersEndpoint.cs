using MapsterMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using FilePocket.Domain.Models;
using static System.Net.WebRequestMethods;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class GetAllFoldersEndpoint : BaseEndpointWithoutRequest<List<FolderModel>>
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

            var response = _mapper.Map<List<FolderModel>>(folders);

            await SendOkAsync(response, cancellationToken);
        }
    }
}

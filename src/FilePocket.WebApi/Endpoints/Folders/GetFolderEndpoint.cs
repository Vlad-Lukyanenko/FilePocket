using MapsterMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using FilePocket.Domain.Models;
using static System.Net.WebRequestMethods;
using FilePocket.Contracts.Folders.Responses;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class GetFolderEndpoint : BaseEndpointWithoutRequest<GetFolderResponse>
    {
        private readonly IServiceManager _service;
        private readonly IMapper _mapper;

        public GetFolderEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/pockets/{pocketId:guid}/folders/{folderId:guid}");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var folder = await _service.FolderService.GetAsync(FolderId!.Value);
            var response = _mapper.Map<GetFolderResponse>(folder);

            await SendOkAsync(response, cancellationToken);
        }
    }
}

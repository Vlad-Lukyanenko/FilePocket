using MapsterMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using FilePocket.Domain.Models;
using FilePocket.Contracts.Folders.Requests;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class CreateEndpoint : BaseEndpointWithoutResponse<CreateEndpointRequest>
    {
        private readonly IServiceManager _service;
        private readonly IMapper _mapper;

        public CreateEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Post("api/folders");
        }

        public override async Task HandleAsync(CreateEndpointRequest request, CancellationToken cancellationToken)
        {
            var folder = _mapper.Map<FolderModel>(request);
            folder.UserId = UserId;
            await _service.FolderService.CreateAsync(folder);

            await SendOkAsync(cancellationToken);
        }
    }
}

using MapsterMapper;
using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;
using FilePocket.Domain.Models;

namespace FilePocket.WebApi.Endpoints.Folders
{
    public class CreateEndpoint : BaseEndpointWithoutResponse<FolderModel>
    {
        private readonly IServiceManager _service;

        public CreateEndpoint(IServiceManager service, IMapper mapper)
        {
            _service = service;
        }

        public override void Configure()
        {
            Post("api/folders");
        }

        public override async Task HandleAsync(FolderModel folder, CancellationToken cancellationToken)
        {
            folder.UserId = UserId;
            await _service.FolderService.CreateAsync(folder);

            await SendOkAsync(cancellationToken);
        }
    }
}

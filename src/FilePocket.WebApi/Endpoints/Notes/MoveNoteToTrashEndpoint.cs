using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Notes
{
    public class MoveNoteToTrashEndpoint: BaseEndpointWithoutRequestAndResponse
    {
        private readonly IServiceManager _service;
        public MoveNoteToTrashEndpoint(IServiceManager service)
        {
            _service = service;
        }
        public override void Configure()
        {
            Put("api/notes/{id:guid}");
            AuthSchemes("Bearer");
        }
        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var id = Route<Guid>("id"); 
            var result = await _service.FileService.MoveToTrash(UserId, id, cancellationToken);

            if (result)
            {
                await SendOkAsync(cancellationToken);
                return;
            }

            await SendNotFoundAsync(cancellationToken);
        }
    }
}

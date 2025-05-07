using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Notes
{
    public class DeleteNoteEndpoint : BaseEndpointWithoutRequestAndResponse
    {
        private readonly IServiceManager _service;
        public DeleteNoteEndpoint(IServiceManager service)
        {
            _service = service;
        }
        public override void Configure()
        {
            Delete("api/notes/{id:guid}/delete-irreversibly");
            AuthSchemes("Bearer");
        }
        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var id = Route<Guid>("id");
            var result = await _service.FileService.RemoveFileAsync(UserId, id, cancellationToken);

            if (result)
            {
                await SendOkAsync(cancellationToken);
                return;
            }

            await SendNotFoundAsync(cancellationToken);
        }
    }
}

using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Notes
{
    public class GetNoteByIdEndpoint : BaseEndpointWithoutRequest<NoteModel>
    {
        private readonly IServiceManager _service;
        public GetNoteByIdEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Get("api/notes/{id:guid}");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {

            var id = Route<Guid>("id");
            var note = await _service.FileService.GetNoteByUserIdAndIdAsync(UserId, id);

            if (note == null)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(note, cancellationToken);
        }
    }
}

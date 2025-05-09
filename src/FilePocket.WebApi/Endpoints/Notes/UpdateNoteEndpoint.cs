using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Notes
{
    public class UpdateNoteEndpoint : BaseEndpointWithoutResponse<NoteModel>
    {
        private readonly IServiceManager _service;

        public UpdateNoteEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Put("api/notes");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(NoteModel note, CancellationToken cancellationToken)
        {

            var result = await _service.FileService.UpdateNoteContentFileAsync(note, cancellationToken);

            await SendOkAsync(result, cancellationToken);
        }
    }
}

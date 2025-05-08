using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Notes
{
    public class CreateNoteEndpoint : BaseEndpointWithoutResponse<NoteCreateModel>
    {
        private readonly IServiceManager _service;

        public CreateNoteEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Post("api/notes");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(NoteCreateModel note, CancellationToken cancellationToken)
        {
            var result = await _service.FileService.CreateNoteContentFileAsync(note, cancellationToken);

            await SendOkAsync(result, cancellationToken);
        }
    }
}

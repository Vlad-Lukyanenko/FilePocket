using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Notes
{
    public class UpdateNoteEndpoint : BaseEndpointWithoutResponse<NoteModel>
    {
        private readonly IFileService _minioFileService;

        public UpdateNoteEndpoint(IFileService minioFileService)
        {
            _minioFileService = minioFileService;
        }

        public override void Configure()
        {
            Put("api/notes");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(NoteModel note, CancellationToken cancellationToken)
        {

            var result = await _minioFileService.UpdateNoteContentFileAsync(note, cancellationToken);

            await SendOkAsync(result, cancellationToken);
        }
    }
}

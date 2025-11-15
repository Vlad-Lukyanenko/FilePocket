using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Notes
{
    public class CreateNoteEndpoint : BaseEndpointWithoutResponse<NoteCreateModel>
    {
        private readonly IFileService _minioFileService;

        public CreateNoteEndpoint(IFileService minioFileService)
        {
            _minioFileService = minioFileService;
        }

        public override void Configure()
        {
            Post("api/notes");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(NoteCreateModel note, CancellationToken cancellationToken)
        {
            var result = await _minioFileService.CreateNoteContentFileAsync(note, cancellationToken);

            await SendOkAsync(result, cancellationToken);
        }
    }
}

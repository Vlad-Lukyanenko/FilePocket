using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;


namespace FilePocket.WebApi.Endpoints.Notes
{
    public class GetNoteEndpoint : BaseEndpointWithoutRequest<NoteModel>
    {
        private readonly IFileService _minioFileService;
        public GetNoteEndpoint(IFileService minioFileService)
        {
            _minioFileService = minioFileService;
        }

        public override void Configure()
        {
            Get("api/notes/{id:guid}");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var id = Route<Guid>("id");
            var note = await _minioFileService.GetNoteByUserIdAndIdAsync(UserId, id);

            if (note == null)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(note, cancellationToken);
        }
    }
}

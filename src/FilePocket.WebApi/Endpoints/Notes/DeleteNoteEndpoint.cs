using FilePocket.Application.Interfaces.Services;
using FilePocket.WebApi.Endpoints.Base;

namespace FilePocket.WebApi.Endpoints.Notes
{
    public class DeleteNoteEndpoint : BaseEndpointWithoutRequestAndResponse
    {
        private readonly IFileService _minoService;
        public DeleteNoteEndpoint(IFileService minioFileService)
        {
            _minoService = minioFileService;
        }
        public override void Configure()
        {
            Delete("api/notes/{id:guid}");
            AuthSchemes("Bearer");
        }
        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var id = Route<Guid>("id");
            var result = await _minoService.RemoveFileAsync(UserId, id, cancellationToken);

            if (result)
            {
                await SendOkAsync(cancellationToken);
                return;
            }

            await SendNotFoundAsync(cancellationToken);
        }
    }
}

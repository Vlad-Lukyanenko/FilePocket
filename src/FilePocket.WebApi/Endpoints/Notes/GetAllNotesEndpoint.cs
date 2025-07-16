using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using static System.Net.WebRequestMethods;



namespace FilePocket.WebApi.Endpoints.Notes
{
    public class GetAllNotesEndpoint : BaseEndpointWithoutRequest<IEnumerable<NoteModel>>
    {
        private readonly IServiceManager _service;
        public GetAllNotesEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/folders/{folderId:guid}/notes", "api/notes");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var notes = await _service.FileService.GetAllNotesMetadataAsync(UserId, FolderId, false);

            if (notes == null)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(notes, cancellationToken);
        }
    }
}

using FastEndpoints;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;


namespace FilePocket.WebApi.Endpoints.Files
{
    public class GetAllFilesEndpoint : BaseEndpoint<GetFilesRequest, IEnumerable<FileResponseModel>>
    {
        private readonly IServiceManager _service;
        public GetAllFilesEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Get("api/pockets/{pocketId:guid}/folders/{folderId:guid}/{isSoftDeleted:bool}/files",
                "api/pockets/{pocketId:guid}/{isSoftDeleted:bool}/files");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(GetFilesRequest request, CancellationToken cancellationToken)
        {
            var fileMetadata = await _service.FileService.GetAllFilesMetadataAsync(UserId, 
                request.PocketId, 
                request.FolderId, 
                request.IsSoftDeleted);

            if (fileMetadata == null)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(fileMetadata, cancellationToken);
        }


    }
    public class GetFilesRequest
    {
        [BindFrom("pocketId")]
        public Guid PocketId { get; set; }

        [BindFrom("folderId")]
        public Guid? FolderId { get; set; }

        [BindFrom("isSoftDeleted")]
        public bool IsSoftDeleted { get; set; }
    }

}

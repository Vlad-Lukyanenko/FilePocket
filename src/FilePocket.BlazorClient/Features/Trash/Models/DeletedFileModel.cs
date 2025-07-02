using FilePocket.BlazorClient.Features.Search.Models;

namespace FilePocket.BlazorClient.Features.Trash.Models
{
    public class DeletedFileModel : FileSearchResponseModel
    {
        public override string ItemName => OriginalName;
    }
}

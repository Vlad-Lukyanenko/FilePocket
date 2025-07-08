using FilePocket.BlazorClient.Features.Search.Models;

namespace FilePocket.BlazorClient.Features.Trash.Models
{
    public class DeletedFolderModel : FolderSearchResponseModel
    {
        public override string ItemName => Name;
    }
}

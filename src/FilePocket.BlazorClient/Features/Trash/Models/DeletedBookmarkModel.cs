using FilePocket.BlazorClient.Features.Search.Models;

namespace FilePocket.BlazorClient.Features.Trash.Models
{
    public class DeletedBookmarkModel : BookmarkSearchResponseModel
    {
        public override string ItemName => Title;
    }
}

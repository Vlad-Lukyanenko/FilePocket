using FilePocket.BlazorClient.Shared.Enums;

namespace FilePocket.BlazorClient.Features.Search.Models
{
    public class FolderSearchResponseModel : SearchResponseModel
    {
        public string Name { get; set; } = string.Empty;
        public FolderType FolderType { get; set; }
    }
}

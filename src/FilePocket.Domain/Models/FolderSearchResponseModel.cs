using FilePocket.Domain.Enums;

namespace FilePocket.Domain.Models
{
    public class FolderSearchResponseModel : SearchResponseModel
    {
        public string Name { get; set; } = string.Empty;
        public FolderType FolderType { get; set; }
    }
}

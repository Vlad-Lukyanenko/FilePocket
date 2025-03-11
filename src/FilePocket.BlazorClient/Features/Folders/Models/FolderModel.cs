using FilePocket.BlazorClient.Shared.Enums;

namespace FilePocket.BlazorClient.Features.Folders.Models
{
    public class FolderModel
    {
        public Guid? Id { get; set; }

        public Guid? PocketId { get; set; }

        public Guid? ParentFolderId { get; set; }
        
        public string Name { get; set; } = string.Empty;

        public FolderType FolderType { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsSelected { get; set; }
    }
}

using FilePocket.Domain.Enums;

namespace FilePocket.Domain.Entities
{
    public class Folder
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PocketId { get; set; }
        public Guid? ParentFolderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public FolderType FolderType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public virtual ICollection<Bookmark>? Bookmarks { get; set; }
    }
}

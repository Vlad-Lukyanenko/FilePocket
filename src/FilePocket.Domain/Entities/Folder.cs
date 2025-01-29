namespace FilePocket.Domain.Entities
{
    public class Folder
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? PocketId { get; set; }
        public Guid? ParentFolderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

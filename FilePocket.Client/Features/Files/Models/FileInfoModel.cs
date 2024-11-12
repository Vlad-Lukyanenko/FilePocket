namespace FilePocket.Client.Services.Files.Models
{
    public class FileInfoModel
    {
        public Guid Id { get; set; }

        public string? OriginalName { get; set; }

        public string? FileType { get; set; }

        public DateTime DateCreated { get; set; }

        public Guid StorageId { get; set; }
        public Guid? FolderId { get; set; }

        public double FileSize { get; set; }

        public bool IsSelected { get; set; }
    }
}

namespace FilePocket.Admin.Models.Files
{
    public class FileModel
    {
        public Guid Id { get; set; }

        public string? OriginalName { get; set; }

        public string? ActualName { get; set; }

        public string? Path { get; set; }

        public string? FileType { get; set; }

        public DateTime? DateCreated { get; set; } 

        public Guid StorageId { get; set; }

        public double? FileSize { get; set; }

        public string? StorageName { get; set; }
    }
}

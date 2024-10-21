namespace FilePocket.Admin.Models.Files
{
    public class ImageResponseModel
    {
        public byte[]? FileByteArray { get; set; }
        public string? Base64Image { get; set; }
        public string? FileUrl { get; set; }
        public string? OriginalName { get; set; }
        public string? FileType { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid StorageId { get; set; }
        public double? FileSize { get; set; }
        public string? StorageName { get; set; }
    }
}
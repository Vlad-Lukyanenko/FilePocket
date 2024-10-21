namespace FilePocket.Admin.Models
{
    public class SessionModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid StorageId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int ChunkSize { get; set; }

    }
}

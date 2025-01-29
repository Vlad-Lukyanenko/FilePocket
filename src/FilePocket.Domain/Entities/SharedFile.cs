namespace FilePocket.Domain.Entities
{
    public class SharedFile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid FileId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace FilePocket.Domain.Models
{
    public class NoteCreateResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }
    }
}

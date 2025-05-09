using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models
{
    public class NoteModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public Guid PocketId { get; set; }

        public Guid? FolderId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}

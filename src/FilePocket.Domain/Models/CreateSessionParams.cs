using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models
{
    public class CreateSessionParams
    {
        [Required]
        public long? FileSize { get; set; }

        [Required]
        public string? FileName { get; set; }

    }
}

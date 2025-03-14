using FilePocket.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FilePocket.Domain.Models
{
    public class FolderModel
    {
        public Guid? Id { get; set; }

        public Guid UserId { get; set; }
        public Guid? PocketId { get; set; }

        public Guid? ParentFolderId { get; set; }
        [Required(ErrorMessage = "Folder name is required.")]
        public string Name { get; set; } = string.Empty;

        public FolderType FolderType { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}


namespace FilePocket.Domain.Models
{
    public class SearchResponseModel
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Guid PocketId { get; set; }

        public Guid? FolderId { get; set; }

        public Guid UserId { get; set; }
    }
}

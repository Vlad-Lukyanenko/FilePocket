namespace FilePocket.Domain.Entities
{
    public class AccountSettings
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int StorageCapacity { get; set; } // in Mb
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

namespace FilePocket.Admin.Models.Storage
{
    public class AddStorageModel
    {
        public string Name { get; set; } = string.Empty;

        public Guid UserId { get; set; }
    }
}

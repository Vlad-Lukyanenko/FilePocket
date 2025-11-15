namespace FilePocket.Domain.Entities.Abstractions
{
    public interface IBaseMetadata
    {
        public Guid Id { get; init; }
        public string ActualName { get; init; }
        public string Path { get; set; }
    }
}

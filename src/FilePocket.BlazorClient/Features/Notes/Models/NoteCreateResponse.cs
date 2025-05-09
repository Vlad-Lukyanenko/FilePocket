namespace FilePocket.BlazorClient.Features.Notes.Models
{
    public record NoteCreateResponse (Guid Id = default, DateTime CreatedAt = default, DateTime? UpdatedAt = default);

}

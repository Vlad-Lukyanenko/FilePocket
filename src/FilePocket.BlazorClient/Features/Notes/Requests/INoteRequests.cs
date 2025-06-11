using FilePocket.BlazorClient.Features.Notes.Models;

namespace FilePocket.BlazorClient.Features.Notes.Requests
{
    public interface INoteRequests
    {
        Task<List<NoteModel>> GetAllByFolderId(Guid? folderId = null);
        Task<NoteModel> GetByIdAsync(Guid id);
        Task<NoteCreateResponse> CreateAsync(NoteCreateModel note);
        Task<NoteUpdateResponse> UpdateAsync(NoteModel note);
        Task<bool> DeleteAsync(Guid noteId);
        Task<bool> IrreversiblyDeleteAsync(Guid noteId);
    }
}

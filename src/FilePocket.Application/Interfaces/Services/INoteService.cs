using FilePocket.Domain.Models;

namespace FilePocket.Application.Interfaces.Services
{
    public interface INoteService
    {
        Task<IEnumerable<NoteModel>> GetAllByUserIdAndFolderIdAsync(Guid userId, Guid? folderId = null, CancellationToken cancellationToken = default);
        Task<NoteModel> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<NoteCreateResponse> CreateAsync(NoteCreateModel note, CancellationToken cancellationToken = default);
        Task<NoteUpdateResponse> UpdateAsync(NoteModel note, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task BulkDeleteAsync(Guid folderId, CancellationToken cancellationToken = default);
        Task BulkSoftDeleteAsync(Guid folderId, CancellationToken cancellationToken = default);
    }
}

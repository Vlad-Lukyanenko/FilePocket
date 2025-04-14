using FilePocket.Domain.Entities;

namespace FilePocket.Application.Interfaces.Repositories;
public interface INotesRepository
{
    Task<List<Note>> GetAllByUserIdAndFolderIdAsync(Guid userId, Guid? folderId = null, CancellationToken cancellationToken = default);
    Task<Note> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAsync(Note note, CancellationToken cancellationToken = default);
    Task AddEncryptedContent(Note note, CancellationToken cancellationToken = default);
    Task UpdateAsync(Note note, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task BulkDeleteAsync(Guid folderId, CancellationToken cancellationToken = default);
    Task BulkSoftDeleteAsync(Guid folderId, CancellationToken cancellationToken = default);
}

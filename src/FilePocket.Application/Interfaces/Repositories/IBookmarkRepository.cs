using FilePocket.Domain.Entities;

namespace FilePocket.Application.Interfaces.Repositories;

public interface IBookmarkRepository
{
    Task<Bookmark> GetByIdAsync(Guid id);
    IEnumerable<Bookmark> GetAll(Guid userId, bool isSoftDeleted, bool trackChanges);
    Task<List<Bookmark>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool isSoftDeleted, bool trackChanges);
    void CreateBookmark(Bookmark bookmark);
    void DeleteBookmark(Bookmark bookmark);
    Task<List<Bookmark>> GetBookmarksByPartialNameAsync(Guid userId, string partialName, bool trackChanges = false);
    Task<List<Bookmark>> GetAllSoftdeletedAsync(Guid userId, bool trackChanges = false);
    void DeleteBookmarks(IEnumerable<Bookmark> bookmarks);
}

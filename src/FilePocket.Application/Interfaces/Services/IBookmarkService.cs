using FilePocket.Contracts.Bookmark;
using FilePocket.Domain.Models;

namespace FilePocket.Application.Interfaces.Services;

public interface IBookmarkService
{
    IEnumerable<BookmarkModel> GetAll(Guid userId, bool isSoftDeleted, bool trackChanges);
    Task<BookmarkModel> CreateBookmarkAsync(BookmarkModel bookmark);
    Task<IEnumerable<BookmarkModel>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool isSoftDeleted, bool trackChanges);
    Task UpdateBookmarkAsync(UpdateBookmarkRequest bookmark);
    Task DeleteBookmarkAsync(Guid id);
    Task MoveToTrashAsync(Guid id);
    Task RestoreFromTrashAsync(Guid id);
    Task<IEnumerable<BookmarkSearchResponseModel>> SearchAsync(Guid userId, string partialName);
    Task<IEnumerable<DeletedBookmarkModel>> GetAllSoftDeletedAsync(Guid userId);
    Task DeleteAllBookmarksAsync(Guid userId);
    Task<DeletedBookmarkModel> GetSoftDeletedAsync(Guid id);
}

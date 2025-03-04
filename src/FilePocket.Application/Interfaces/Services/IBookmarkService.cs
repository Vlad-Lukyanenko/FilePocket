using FilePocket.Domain.Models;

namespace FilePocket.Application.Interfaces.Services;

public interface IBookmarkService
{
    IEnumerable<BookmarkModel> GetAll(Guid userId, bool trackChanges);
    Task<BookmarkModel> CreateBookmarkAsync(BookmarkModel bookmark);
    Task<IEnumerable<BookmarkModel>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool trackChanges);
    Task UpdateBookmarkAsync(BookmarkModel bookmark);
    Task DeleteBookmarkAsync(Guid id);
}

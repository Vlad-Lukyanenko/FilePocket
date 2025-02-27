using FilePocket.Domain.Models;

namespace FilePocket.Application.Interfaces.Services;

public interface IBookmarkService
{
    IEnumerable<BookmarkModel> GetAll(Guid userId, bool trackChanges);
    Task<BookmarkModel> CreateBookmarkAsync(BookmarkModel bookmark);
    Task UpdateBookmarkAsync(BookmarkModel bookmark);
    Task DeleteBookmarkAsync(Guid id);
}

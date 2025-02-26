using FilePocket.Domain.Models;

namespace FilePocket.Application.Interfaces.Services;

public interface IBookmarkService
{
    Task<BookmarkModel> CreateBookmarkAsync(BookmarkModel bookmark);
    Task DeleteBookmarkAsync(Guid id);
}

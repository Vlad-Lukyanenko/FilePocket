using FilePocket.Domain.Entities;

namespace FilePocket.Application.Interfaces.Repositories;

public interface IBookmarkRepository
{
    Task<Bookmark> GetByIdAsync(Guid id);
    void CreateBookmark(Bookmark bookmark);
    void DeleteBookmark(Bookmark bookmark);
}

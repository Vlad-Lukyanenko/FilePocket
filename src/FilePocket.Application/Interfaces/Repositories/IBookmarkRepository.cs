using FilePocket.Domain.Entities;

namespace FilePocket.Application.Interfaces.Repositories;

public interface IBookmarkRepository
{
    Task<Bookmark> GetByIdAsync(Guid id);
    IEnumerable<Bookmark> GetAll(Guid userId, bool trackChanges);
    void CreateBookmark(Bookmark bookmark);
    void DeleteBookmark(Bookmark bookmark);
}

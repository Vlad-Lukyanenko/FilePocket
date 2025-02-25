using FilePocket.Domain.Entities;

namespace FilePocket.Application.Interfaces.Repositories;

public interface IBookmarkRepository
{
    void CreateBookmark(Bookmark bookmark);
}

using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain.Entities;

namespace FilePocket.Persistence.Repositories;

public class BookmarkRepository : RepositoryBase<Bookmark>, IBookmarkRepository
{
    public BookmarkRepository(FilePocketDbContext context)
        : base(context)
    {
    }

    public void CreateBookmark(Bookmark bookmark)
    {
        Create(bookmark);
    }
}

using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.Persistence.Repositories;

public class BookmarkRepository : RepositoryBase<Bookmark>, IBookmarkRepository
{
    public BookmarkRepository(FilePocketDbContext context)
        : base(context)
    {
    }

    public IEnumerable<Bookmark> GetAll(Guid userId, bool trackChanges)
    {
        return FindByCondition(b => b.UserId == userId, trackChanges).OrderByDescending(b => b.CreatedAt);
    }

    public async Task<Bookmark> GetByIdAsync(Guid id)
    {
        return (await FindByCondition(b => b.Id.Equals(id)))!;
    }

    public async Task<List<Bookmark>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool trackChanges)
    {
        return await FindByCondition(b => b.UserId.Equals(userId) && b.PocketId.Equals(pocketId) && b.FolderId.Equals(folderId), trackChanges).ToListAsync();
    }

    public void CreateBookmark(Bookmark bookmark)
    {
        Create(bookmark);
    }

    public void DeleteBookmark(Bookmark bookmark)
    {
        Delete(bookmark);
    }
}

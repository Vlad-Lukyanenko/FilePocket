using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.Infrastructure.Persistence.Repositories;

public class BookmarkRepository : RepositoryBase<Bookmark>, IBookmarkRepository
{
    public BookmarkRepository(FilePocketDbContext context)
        : base(context)
    {
    }

    public IEnumerable<Bookmark> GetAll(Guid userId, bool isSoftDeleted, bool trackChanges)
    {
        return FindByCondition(b => b.UserId == userId && b.IsDeleted == isSoftDeleted, trackChanges).OrderByDescending(b => b.CreatedAt);
    }

    public async Task<Bookmark> GetByIdAsync(Guid id)
    {
        return (await FindByCondition(b => b.Id.Equals(id)))!;
    }

    public async Task<List<Bookmark>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool isSoftDeleted, bool trackChanges)
    {
        return await FindByCondition(b => b.UserId.Equals(userId)
                                            && b.PocketId.Equals(pocketId)
                                            && b.FolderId.Equals(folderId)
                                            && b.IsDeleted == isSoftDeleted, trackChanges).ToListAsync();
    }

    public IEnumerable<Bookmark> GetAllByFolderId(Guid folderId, bool trackChanges = false)
    {
        return FindByCondition(b => b.FolderId.Equals(folderId), trackChanges);
    }

    public void CreateBookmark(Bookmark bookmark)
    {
        Create(bookmark);
    }

    public void UpdateBookmark(Bookmark bookmark)
    {
        Update(bookmark);
    }

    public void DeleteBookmark(Bookmark bookmark)
    {
        Delete(bookmark);
    }

    public async Task<List<Bookmark>> GetBookmarksByPartialNameAsync(Guid userId, string partialName, bool trackChanges = false)
    {
        return await FindByCondition(f => f.UserId.Equals(userId) && f.Title.ToLower().Contains(partialName.ToLower()), trackChanges)
            .ToListAsync();
    }

    public async Task<List<Bookmark>> GetAllSoftdeletedAsync(Guid userId, bool trackChanges = false)
    {
        return await FindByCondition(f => f.UserId.Equals(userId) && f.IsDeleted, trackChanges)
            .ToListAsync();
    }

    public void DeleteBookmarks(IEnumerable<Bookmark> bookmarks)
    {
        DeleteAll(bookmarks);
    }
}

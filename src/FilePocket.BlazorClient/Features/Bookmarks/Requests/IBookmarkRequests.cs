using FilePocket.BlazorClient.Features.Bookmarks.Models;

namespace FilePocket.BlazorClient.Features.Bookmarks.Requests;

public interface IBookmarkRequests
{
    Task<List<BookmarkModel>> GetAllAsync(Guid? pocketId, Guid? folderId);
    Task<bool> CreateAsync(CreateBookmarkModel bookmark);
    Task<bool> UpdateAsync(UpdateBookmarkModel bookmark);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> SoftDeleteAsync(Guid id);
}

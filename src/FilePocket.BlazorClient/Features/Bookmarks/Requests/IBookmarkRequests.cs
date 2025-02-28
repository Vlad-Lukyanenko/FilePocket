using FilePocket.BlazorClient.Features.Bookmarks.Models;

namespace FilePocket.BlazorClient.Features.Bookmarks.Requests;

public interface IBookmarkRequests
{
    Task<IEnumerable<BookmarkModel>> GetAllAsync();
    Task<bool> CreateAsync(CreateBookmarkModel bookmark);
    Task<bool> UpdateAsync(UpdateBookmarkModel bookmark);
    Task<bool> DeleteAsync(Guid id);
}

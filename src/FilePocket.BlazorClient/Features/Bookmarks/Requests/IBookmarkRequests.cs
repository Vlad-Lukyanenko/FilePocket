using FilePocket.BlazorClient.Features.Bookmarks.Models;

namespace FilePocket.BlazorClient.Features.Bookmarks.Requests;

public interface IBookmarkRequests
{
    Task<List<BookmarkModel>> GetAllAsync(Guid? pocketId, Guid? folderId, bool isSoftDeleted);
    Task<bool> CreateAsync(CreateBookmarkModel bookmark);
    Task<UpdateBookmarkResponseModel> UpdateAsync(UpdateBookmarkModel bookmark);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> SoftDeleteAsync(Guid id);
}

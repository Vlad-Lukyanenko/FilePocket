using FilePocket.BlazorClient.Features.Bookmarks.Models;
using FilePocket.BlazorClient.Features.Bookmarks.Requests;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.Pages.Bookmarks;

public partial class Bookmarks
{
    private List<BookmarkModel> _bookmarks = new List<BookmarkModel>();
    private bool _loading = true;
    private Guid _bookmarkIdToBeDeleted;

    [Parameter] public Guid PocketId { get; set; }
    [Parameter] public Guid? FolderId { get; set; }

    [Inject] private IPocketRequests PocketRequests { get; set; } = default!;
    [Inject] private IBookmarkRequests BookmarkRequests { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        _bookmarks = (await BookmarkRequests.GetAllAsync()).ToList();
        _loading = false;

        if (PocketId == Guid.Empty)
        {
            var defaultPocket = await PocketRequests.GetDefaultAsync();
            PocketId = defaultPocket.Id;
        }
    }

    private string GetCreateBookmarkUrl()
    {
        if (FolderId is null)
        {
            return $"/pockets/{PocketId}/bookmarks/new";
        }

        return $"/pockets/{PocketId}/folders/{FolderId}/bookmarks/new";
    }

    private void RemoveClicked(BookmarkModel bookmark)
    {
        _bookmarkIdToBeDeleted = bookmark.Id;
    }

    private async Task ConfirmDeletionClickedAsync()
    {
        var bookmark = _bookmarks.FirstOrDefault(b => b.Id == _bookmarkIdToBeDeleted);

        if (bookmark is not null)
        {
            var isDeleted = await BookmarkRequests.DeleteAsync(bookmark.Id);

            if (isDeleted)
            {
                _bookmarks.Remove(bookmark);
                _bookmarkIdToBeDeleted = default;
            }
        }
    }

    private void CancelDeletionClicked()
    {
        _bookmarkIdToBeDeleted = default;
    }
}

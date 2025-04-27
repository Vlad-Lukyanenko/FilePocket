using FilePocket.BlazorClient.Features.Bookmarks.Models;
using FilePocket.BlazorClient.Features.Bookmarks.Requests;
using FilePocket.BlazorClient.Features.Folders.Models;
using FilePocket.BlazorClient.Services.Folders.Requests;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using FilePocket.BlazorClient.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;

namespace FilePocket.BlazorClient.Pages.Bookmarks;

public partial class Bookmarks
{
    private List<BookmarkModel> _bookmarks = new();
    private ObservableCollection<FolderModel> _folders = new();
    private bool _loading = true;
    private Guid _bookmarkIdToBeDeleted;
    private Guid _bookmarkIdToBeUpdated;
    private Dictionary<string, string> _oldbookMarkValues = new();
    private string _pageTitle = string.Empty;
    private string _createFolderUrl = string.Empty;
    private FolderModel? _currentFolder;
    private bool _deleteFolderStarted;

    [Parameter] public Guid PocketId { get; set; }
    [Parameter] public Guid? FolderId { get; set; }

    [Inject] private IPocketRequests PocketRequests { get; set; } = default!;
    [Inject] private IBookmarkRequests BookmarkRequests { get; set; } = default!;
    [Inject] private IFolderRequests FolderRequests { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        List<FolderModel> folders;
        var folderTypes = new List<FolderType> { FolderType.Bookmarks };

        if (PocketId == Guid.Empty)
        {
            var defaultPocket = await PocketRequests.GetDefaultAsync();
            PocketId = defaultPocket.Id;
        }

        _currentFolder = FolderId is null ? null : await FolderRequests.GetAsync(PocketId, FolderId.Value);
        var currentFolderName = _currentFolder is null ? string.Empty : $" - {_currentFolder.Name}";
        var currentFolderUrl = FolderId is null ? "" : $"{FolderId}/";

        _pageTitle = $"My bookmarks{currentFolderName}";

        _createFolderUrl = $"/pockets/{PocketId}/folders/{currentFolderUrl}{(int)FolderType.Bookmarks}/new";

        if (FolderId is null)
        {
            folders = (await FolderRequests.GetAllAsync(PocketId, folderTypes, isSoftDeleted: false)).ToList();
        }
        else
        {
            folders = (await FolderRequests.GetAllAsync(PocketId, FolderId.Value, folderTypes, isSoftDeleted: false)).ToList();
        }

        _folders = new ObservableCollection<FolderModel>(folders);
        _bookmarks = await BookmarkRequests.GetAllAsync(PocketId, FolderId, isSoftDeleted: false);
        _loading = false;
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
        var bookmarkToUpdate = _bookmarks.FirstOrDefault(b => b.Id == _bookmarkIdToBeUpdated);

        if (bookmarkToUpdate is not null)
        {
            ReturnOldBookmarkValues(bookmarkToUpdate);
            RemoveOldBookmarkValues(bookmarkToUpdate.Id);
        }

        _bookmarkIdToBeDeleted = bookmark.Id;
        _bookmarkIdToBeUpdated = default;
    }

    private void UpdateClicked(BookmarkModel bookmark)
    {
        var bookmarkToUpdate = _bookmarks.FirstOrDefault(b => b.Id == _bookmarkIdToBeUpdated);

        if (bookmarkToUpdate is not null)
        {
            ReturnOldBookmarkValues(bookmarkToUpdate);
            RemoveOldBookmarkValues(bookmarkToUpdate.Id);
        }

        _oldbookMarkValues.Add($"{bookmark.Id}title", bookmark.Title);
        _oldbookMarkValues.Add($"{bookmark.Id}url", bookmark.Url);
        _bookmarkIdToBeUpdated = bookmark.Id;
        _bookmarkIdToBeDeleted = default;
    }

    private async Task ConfirmDeletionClickedAsync()
    {
        var bookmark = _bookmarks.FirstOrDefault(b => b.Id == _bookmarkIdToBeDeleted);

        if (bookmark is not null)
        {
            var isDeleted = await BookmarkRequests.SoftDeleteAsync(bookmark.Id);

            if (isDeleted)
            {
                _bookmarks.Remove(bookmark);
                _bookmarkIdToBeDeleted = default;
            }
        }
    }

    private async Task ConfirmUpdatingClickedAsync()
    {
        var bookmark = _bookmarks.FirstOrDefault(b => b.Id == _bookmarkIdToBeUpdated);

        if (bookmark is not null)
        {
            if (string.IsNullOrEmpty(bookmark.Title) || string.IsNullOrEmpty(bookmark.Url))
            {
                return;
            }

            var bookmarkToUpdate = new UpdateBookmarkModel
            {
                Id = bookmark.Id,
                PocketId = bookmark.PocketId,
                FolderId = bookmark.FolderId,
                Title = bookmark.Title,
                Url = bookmark.Url,
                UserId = bookmark.UserId,
                IsDeleted = bookmark.IsDeleted
            };

            var isUpdated = await BookmarkRequests.UpdateAsync(bookmarkToUpdate);

            if (isUpdated)
            {
                _bookmarkIdToBeUpdated = default;
                RemoveOldBookmarkValues(bookmark.Id);
            }
        }
    }

    private void CancelClicked()
    {
        var bookmark = _bookmarks.FirstOrDefault(b => b.Id == _bookmarkIdToBeUpdated);

        if (bookmark is not null)
        {
            ReturnOldBookmarkValues(bookmark);
            RemoveOldBookmarkValues(bookmark.Id);
        }

        _bookmarkIdToBeDeleted = default;
        _bookmarkIdToBeUpdated = default;
    }

    private void RemoveOldBookmarkValues(Guid id)
    {
        _oldbookMarkValues.Remove($"{id}title");
        _oldbookMarkValues.Remove($"{id}url");
    }

    private void ReturnOldBookmarkValues(BookmarkModel bookmark)
    {
        var oldBookmarkTitle = _oldbookMarkValues.First(b => b.Key.Equals($"{bookmark.Id}title")).Value;
        var oldBookmarkUrl = _oldbookMarkValues.First(b => b.Key.Equals($"{bookmark.Id}url")).Value;
        bookmark.Title = oldBookmarkTitle;
        bookmark.Url = oldBookmarkUrl;
    }

    private async Task DeleteFolderClickAsync()
    {
        if (FolderId is not null)
        {
            await FolderRequests.SoftDeleteAsync(FolderId.Value);
        }

        _deleteFolderStarted = false;

        await JSRuntime.InvokeVoidAsync("history.back");
    }
}

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

public partial class DeletedBookmarks
{
    private List<BookmarkModel> _bookmarks = new();
    private ObservableCollection<FolderModel> _folders = new();
    private bool _loading = true;
    private Guid _bookmarkIdToBeDeleted;
    private Guid _bookmarkIdToBeRestored;
    private string _pageTitle = string.Empty;
    private FolderModel? _currentFolder;
    private bool _deleteFolderStarted;
    private bool _restoreFolderStarted;

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

        _currentFolder = FolderId is null ? null : await FolderRequests.GetAsync(FolderId.Value, PocketId);
        var currentFolderName = _currentFolder is null ? string.Empty : $" - {_currentFolder.Name}";
        var currentFolderUrl = FolderId is null ? "" : $"{FolderId}/";

        _pageTitle = $"Deleted bookmarks{currentFolderName}";

        if (PocketId == Guid.Empty)
        {
            var defaultPocket = await PocketRequests.GetDefaultAsync();
            PocketId = defaultPocket.Id;
        }

        if (FolderId is null)
        {
            folders = (await FolderRequests.GetAllAsync(PocketId, folderTypes, isSoftDeleted: true)).ToList();
        }
        else
        {
            folders = (await FolderRequests.GetAllAsync(PocketId, FolderId.Value, folderTypes, isSoftDeleted: true)).ToList();
        }

        _folders = new ObservableCollection<FolderModel>(folders);
        _bookmarks = await BookmarkRequests.GetAllAsync(PocketId, FolderId, isSoftDeleted: true);
        _loading = false;
    }

    private void RemoveClicked(BookmarkModel bookmark)
    {
        _bookmarkIdToBeDeleted = bookmark.Id;
        _bookmarkIdToBeRestored = default;
    }

    private void RestoreClicked(BookmarkModel bookmark)
    {
        _bookmarkIdToBeRestored = bookmark.Id;
        _bookmarkIdToBeDeleted = default;
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

    private async Task ConfirmRestorationClickedAsync()
    {
        var bookmark = _bookmarks.FirstOrDefault(b => b.Id == _bookmarkIdToBeRestored);

        if (bookmark is not null)
        {
            var bookmarkToUpdate = new UpdateBookmarkModel
            {
                Id = bookmark.Id,
                PocketId = bookmark.PocketId,
                FolderId = null,
                Title = bookmark.Title,
                Url = bookmark.Url,
                UserId = bookmark.UserId,
                IsDeleted = false
            };

            var isRestored = await BookmarkRequests.UpdateAsync(bookmarkToUpdate);

            if (isRestored)
            {
                _bookmarkIdToBeRestored = default;
                _bookmarks.Remove(bookmark);
            }
        }
    }

    private void CancelClicked()
    {
        _bookmarkIdToBeDeleted = default;
        _bookmarkIdToBeRestored = default;
    }

    private async Task DeleteFolderClickAsync()
    {
        if (FolderId is not null)
        {
            await FolderRequests.DeleteAsync(FolderId.Value);
        }

        _deleteFolderStarted = false;

        await JSRuntime.InvokeVoidAsync("history.back");
    }

    private async Task RestoreFolderClickAsync()
    {
        if (FolderId is not null)
        {
            await FolderRequests.RestoreAsync(FolderId.Value);
        }

        _restoreFolderStarted = false;

        await JSRuntime.InvokeVoidAsync("history.back");        
    }
}

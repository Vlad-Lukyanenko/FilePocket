using FilePocket.BlazorClient.Features.Bookmarks.Requests;
using FilePocket.BlazorClient.Shared.Enums;
using FilePocket.BlazorClient.Features.Trash.Models;
using FilePocket.BlazorClient.Features.Trash.Requests;
using FilePocket.BlazorClient.Services.Files.Requests;
using FilePocket.BlazorClient.Services.Folders.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.Pages;

public partial class Trash
{
    private bool _clearTrashStarted;
    private List<DeletedFileModel> _files = [];
    private List<DeletedFolderModel> _folders = [];
    private List<DeletedBookmarkModel> _bookmarks = [];
    private readonly FileTypes[] _fileTypes = Enum.GetValues<FileTypes>();
    private readonly FolderType[] _folderTypes = Enum.GetValues<FolderType>();
    private bool _isLoading = true;
    private const int ItemsToShowThreshold = 5;

    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ITrashRequests TrashRequests { get; set; } = default!;
    [Inject] private IFileRequests FileRequests { get; set; } = default!;
    [Inject] private IFolderRequests FolderRequests { get; set; } = default!;
    [Inject] private IBookmarkRequests BookmarkRequests { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;

        await Task.WhenAll(
            SetFiles(),
            SetFolders(),
            SetBookmarks()
            );

        _isLoading = false;
    }

    private async Task ClearTrashClickAsync()
    {
        await TrashRequests.ClearAllTrashAsync();

        Navigation.NavigateTo("/");
    }

    private void Navigate(string uri)
    {
        Navigation.NavigateTo(uri);
    }

    private async Task SetFiles()
    {
        _files = await TrashRequests.GetAllSoftdelted<DeletedFileModel>(RequestedItemType.File);
    }

    private async Task SetFolders()
    {
        _folders = await TrashRequests.GetAllSoftdelted<DeletedFolderModel>(RequestedItemType.Folder);
    }

    private async Task SetBookmarks()
    {
        _bookmarks = await TrashRequests.GetAllSoftdelted<DeletedBookmarkModel>(RequestedItemType.Bookmark);
    }

    private static string GetCollapseClass(int itemsCount)
    {
        return itemsCount > ItemsToShowThreshold ? "collapse" : "collapse show";
    }
}

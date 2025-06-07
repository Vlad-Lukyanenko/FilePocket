using FilePocket.BlazorClient.Features.Search.Enums;
using FilePocket.BlazorClient.Features.Search.Models;
using FilePocket.BlazorClient.Features.Search.Requests;
using FilePocket.BlazorClient.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.Pages
{
    public partial class SearchResults
    {

        private List<FileSearchResponseModel> _files = [];
        private List<FolderSearchResponseModel> _folders = [];
        private List<BookmarkSearchResponseModel> _bookmarks = [];
        private readonly FileTypes[] _fileTypes = Enum.GetValues<FileTypes>();
        private readonly FolderType[] _folderTypes = Enum.GetValues<FolderType>();
        private bool _isLoading = true;
        private const int ItemsToShowThreshold = 5;

        [Parameter] public string PartialName { get; set; } = string.Empty;
        [Inject] private ISearchRequests SearchRequests { get; set; } = default!;

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

        private async Task SetFiles()
        {
            _files = await SearchRequests.GetItemsByPartialNameAsync<FileSearchResponseModel>(RequestedItemType.File, PartialName) ?? [];
        }

        private async Task SetFolders()
        {
            _folders = await SearchRequests.GetItemsByPartialNameAsync<FolderSearchResponseModel>(RequestedItemType.Folder, PartialName) ?? [];
        }

        private async Task SetBookmarks()
        {
            _bookmarks = await SearchRequests.GetItemsByPartialNameAsync<BookmarkSearchResponseModel>(RequestedItemType.Bookmark, PartialName) ?? [];
        }

        private static string GetCollapseClass(int itemsCount)
        {
            return itemsCount > ItemsToShowThreshold ? "collapse" : "collapse show";
        }
    }
}

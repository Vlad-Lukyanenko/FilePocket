using FilePocket.BlazorClient.Features.Files.Models;
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
        [Inject] private ISearchRequests FileSearchRequests { get; set; } = default!;

        protected override async Task OnParametersSetAsync()
        {
            _isLoading = true;
            _files = await FileSearchRequests.GetItemsByPartialNameAsync<FileSearchResponseModel>(SearchItemType.File, PartialName) ?? [];
            _folders = await FileSearchRequests.GetItemsByPartialNameAsync<FolderSearchResponseModel>(SearchItemType.Folder, PartialName) ?? [];
            _bookmarks = await FileSearchRequests.GetItemsByPartialNameAsync<BookmarkSearchResponseModel>(SearchItemType.Bookmark, PartialName) ?? [];
            _isLoading = false;
        }

        private static string GetCollapseClass(int itemsCount)
        {
            return itemsCount > ItemsToShowThreshold ? "collapse" : "collapse show";
        }
    }
}

using FilePocket.BlazorClient.Features.Files.Models;
using FilePocket.BlazorClient.Features.FileSearch.Models;
using FilePocket.BlazorClient.Features.FileSearch.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.Pages
{
    public partial class SearchResults
    {

        private List<FileSearchResponseModel> _files = [];

        private readonly FileTypes[] _fileTypes = Enum.GetValues<FileTypes>();
        [Parameter] public string PartialName { get; set; } = string.Empty;
        [Inject] private IFileSearchRequests FileSearchRequests { get; set; } = default!;

        protected override async Task OnParametersSetAsync()
        {
            _files = await FileSearchRequests.GetFilesByPartialNameAsync(PartialName);
        }
    }
}

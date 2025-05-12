using FilePocket.BlazorClient.Features.Files.Models;
using FilePocket.BlazorClient.Services.Files.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.Pages
{
    public partial class SearchResults
    {

        private List<FileSearchResponseModel> _files = [];

        private readonly FileTypes[] _fileTypes = Enum.GetValues<FileTypes>();
        [Parameter] public string PartialNameToSearch { get; set; } = string.Empty;
        [Inject] private IFileRequests FileRequests { get; set; } = default!;

        protected override async Task OnParametersSetAsync()
        {
            _files = await FileRequests.GetFilesByPartialNameAsync(PartialNameToSearch);
        }
    }
}

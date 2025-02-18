using FilePocket.BlazorClient.Features.Files.Models;
using FilePocket.BlazorClient.Features.SharedFiles.Models;
using FilePocket.BlazorClient.Features.SharedFiles.Requests;
using FilePocket.BlazorClient.Features.Users.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FilePocket.BlazorClient.Pages.Files.Shared
{
    public partial class SharedFiles
    {
        [Inject]
        private ISharedFilesRequests _sharedFilesRequests { get; set; } = default!;

        [Inject]
        private IUserRequests _userRequests { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private List<SharedFileView> _sharedFiles = [];

        protected override async Task OnInitializedAsync()
        {
            _sharedFiles = await _sharedFilesRequests.GetAllAsync();
        }

        private async Task DeleteSharedFile(Guid sharedFileId)
        {
            var response = await _sharedFilesRequests.DeleteAsync(sharedFileId);

            if (response)
            {
                var item = _sharedFiles.FirstOrDefault(c => c.SharedFileId == sharedFileId);

                if (item is not null)
                {
                    _sharedFiles.Remove(item);
                    StateHasChanged();
                }
            }
        }

        private async void CopyIdToClipboard(string sharedFileId)
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", sharedFileId);
        }

        private string TruncateFileName(string fullName, int maxLen = 15)
        {
            var ext = Path.GetExtension(fullName);
            var name = Path.GetFileNameWithoutExtension(fullName);

            if (name.Length > maxLen)
            {
                name = name.Substring(0, maxLen) + "...";
            }

            return name + ext;
        }
    }
}

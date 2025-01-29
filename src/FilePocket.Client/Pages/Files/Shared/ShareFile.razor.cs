using FilePocket.Client.Features.SharedFiles.Models;
using FilePocket.Client.Features.SharedFiles.Requests;
using FilePocket.Client.Services.Files.Models;
using FilePocket.Client.Services.Files.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace FilePocket.Client.Pages.Files.Shared
{
    public partial class ShareFile
    {
        [Parameter]
        public string PocketId { get; set; } = string.Empty;

        [Parameter]
        public string FileId { get; set; } = string.Empty;

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        private ISharedFilesRequests SharedFilesRequests { get; set; } = default!;

        [Inject]
        private IFileRequests FileRequests { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private string _sharedFileUrl = string.Empty;

        private Guid _pocketId;
        private Guid _fileId;

        private FileModel? _fileInfo;

        protected override async Task OnInitializedAsync()
        {
            _fileId = Guid.Parse(FileId);
            _fileInfo = await FileRequests.GetFileInfoAsync(_fileId);
        }

        private async Task ShareFileAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authState.User.Identity?.Name!;

            var sharedFile = new SharedFileModel
            {
                Id = Guid.NewGuid(),
                FileId = Guid.Parse(FileId),
                UserName = userName,
                CreatedAt = DateTime.UtcNow,
            };

            var sharedFileId = await SharedFilesRequests.CreateAsync(sharedFile);

            _sharedFileUrl = $"{Navigation.BaseUri}files/{sharedFileId}/shared";
        }

        private async void CopyIdToClipboard(string sharedFileId)
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", sharedFileId);
        }
    }
}

using FilePocket.BlazorClient.Features.SharedFiles.Models;
using FilePocket.BlazorClient.Features.SharedFiles.Requests;
using FilePocket.BlazorClient.Helpers;
using FilePocket.BlazorClient.Services.Files.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using FilePocket.BlazorClient.Services.Files.Models;
using FilePocket.BlazorClient.Shared.Enums;

namespace FilePocket.BlazorClient.Pages.Files.Shared
{
    public partial class SharedFileInfo
    {
        [Parameter]
        public string SharedFileId { get; set; } = string.Empty;

        [Inject]
        private ISharedFilesRequests _sharedFilesRequests { get; set; } = default!;

        [Inject]
        private IFileRequests FileRequests { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private Guid _sharedFileId;
        private SharedFileModel? _sharedFile = null;
        private string _imageContent = string.Empty;
        private FileModel? _file;

        protected override async Task OnInitializedAsync()
        {
            _sharedFileId = Guid.Parse(SharedFileId);

            _sharedFile = await _sharedFilesRequests.GetByIdAsync(_sharedFileId);

            if (_sharedFile is not null && _sharedFile.FileType == FileTypes.Image)
            {
                _file = await FileRequests.GetImageThumbnailAsync(_sharedFile.FileId, 500);

                string base64 = Convert.ToBase64String(new ReadOnlySpan<byte>(_file.FileByteArray!));
                var mimeType = Tools.GetMimeType(_file.OriginalName!);
                _imageContent = $"data:{mimeType};base64,{base64}";
            }
        }

        private string GetLink()
        {
            return $"{Navigation.BaseUri}files/{_sharedFileId}/shared";
        }

        private async void CopyIdToClipboard()
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", GetLink());
        }
    }
}

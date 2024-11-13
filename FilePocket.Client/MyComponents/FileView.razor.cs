using FilePocket.Client.Services.Files.Models;
using FilePocket.Client.Services.Files.Requests;
using FilePocket.Client.Services.Folders.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FilePocket.Client.MyComponents
{
    public partial class FileView
    {
        [Parameter]
        public string PocketId { get; set; } = string.Empty;

        [Parameter]
        public string FolderId { get; set; } = string.Empty;

        [Parameter]
        public string FileId { get; set; } = string.Empty;

        [Inject]
        private IFileRequests FileRequests { get; set; } = default!;

        [Inject]
        private IFolderRequests FolderRequests { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private Guid _pocketId;
        private Guid _fileId;

        private FileModel? _file;
        private string _imageContent = string.Empty;
        private string _goBackUrl = string.Empty;

        private void InitGoBackUrl()
        {
             _goBackUrl = string.IsNullOrWhiteSpace(FolderId)
                ? $"/pockets/{PocketId}/files"
                : $"/pockets/{PocketId}/folders/{FolderId}/files";
        }

        protected override async Task OnInitializedAsync()
        {
            _pocketId = Guid.Parse(PocketId);
            _fileId = Guid.Parse(FileId);

            _file = await FileRequests.GetFileInfoAsync(_pocketId, _fileId);

            if (_file.FileType == "Image")
            {
                _file = await FileRequests.GetImageThumbnailAsync(_pocketId, _fileId, 700);

                var base64 = Convert.ToBase64String(_file.FileByteArray!);
                _imageContent = $"data:image/jpeg;base64,{base64}";
            }

            InitGoBackUrl();
        }

        private async void DownloadFile()
        {
            var base64 = Convert.ToBase64String(_file!.FileByteArray!);
            await JSRuntime.InvokeVoidAsync("saveFile", _file!.OriginalName, base64);
        }
    }
}

using FilePocket.Client.Services.Files.Models;
using FilePocket.Client.Services.Files.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FilePocket.Client.Pages.Files
{
    public partial class File
    {
        [Parameter]
        public string PocketIdParam { get; set; } = string.Empty;

        [Parameter]
        public string FileIdParam { get; set; } = string.Empty;

        [Inject]
        private IFileRequests FileRequests { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private Guid _pocketId;
        private Guid _fileId;

        private FileModel? _file;
        private string _imageContent = string.Empty;


        protected override async Task OnInitializedAsync()
        {
            _pocketId = Guid.Parse(PocketIdParam);
            _fileId = Guid.Parse(FileIdParam);

            _file = await FileRequests.GetFileInfoAsync(_pocketId, _fileId);

            if (_file.FileType == "Image")
            {
                //_file = await FileRequests.GetFileAsync(_pocketId, _fileId);
                _file = await FileRequests.GetImageThumbnailAsync(_pocketId, _fileId, 700);

                var base64 = Convert.ToBase64String(_file.FileByteArray!);
                _imageContent = $"data:image/jpeg;base64,{base64}";
            }
        }

        private async void DownloadFile()
        {
            var base64 = Convert.ToBase64String(_file!.FileByteArray!);
            await JSRuntime.InvokeVoidAsync("saveFile", _file!.OriginalName, base64);
        }
    }
}

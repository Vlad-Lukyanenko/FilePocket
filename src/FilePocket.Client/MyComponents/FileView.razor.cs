using FilePocket.Client.Features.Files.Models;
using FilePocket.Client.Features.Trash;
using FilePocket.Client.Helpers;
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
        private ITrashRequests TrashRequests { get; set; } = default!;

        [Inject]
        private IFolderRequests FolderRequests { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private Guid _fileId;

        private FileModel? _file;
        private string _imageContent = string.Empty;
        private string _goBackUrl = string.Empty;
        private bool _isLoading = true;
        private bool _isPageLoaded = false;
        private bool _removalProcessStarted = false;

        protected override async Task OnInitializedAsync()
        {
            _fileId = Guid.Parse(FileId);

            _file = await FileRequests.GetFileInfoAsync(_fileId);
            _isLoading = false;

            StateHasChanged();

            if (_file.FileType == FileTypes.Image)
            {
                _file = await FileRequests.GetImageThumbnailAsync(_fileId, 500);

                //string base64 = Convert.ToBase64String(new ReadOnlySpan<byte>(_file.FileByteArray!));
                //var mimeType = Tools.GetMimeType(_file.OriginalName!);
                //_imageContent = $"data:{mimeType};base64,{base64}";

                _imageContent = _file.FileData!;
            }

            _isPageLoaded = true;
            StateHasChanged();

            InitGoBackUrl();
        }

        private void InitGoBackUrl()
        {
            if (string.IsNullOrWhiteSpace(PocketId))
            {
                _goBackUrl = string.IsNullOrWhiteSpace(FolderId)
                   ? $"/files"
                   : $"/folders/{FolderId}/files";
            }
            else
            {
                _goBackUrl = string.IsNullOrWhiteSpace(FolderId)
                   ? $"/pockets/{PocketId}/files"
                   : $"/pockets/{PocketId}/folders/{FolderId}/files";
            }

        }

        private string ShareFileUrl()
        {
            return string.IsNullOrWhiteSpace(PocketId)
                ? $"files/{FileId}/share"
                : $"pockets/{PocketId}/files/{FileId}/share";
        }

        private async void DownloadFile()
        {
            var file = await FileRequests.GetFileAsync(_fileId);
            var base64 = Convert.ToBase64String(file!.FileByteArray!);
            var mimeType = Tools.GetMimeType(file.OriginalName!);
            await JSRuntime.InvokeVoidAsync("saveFile", file.OriginalName, mimeType, base64);
        }

        private async void MoveToTrashClick()
        {
            _removalProcessStarted = true;
            await TrashRequests.MoveFileToTrash(_fileId);

            Navigation.NavigateTo(_goBackUrl);
        }
    }
}

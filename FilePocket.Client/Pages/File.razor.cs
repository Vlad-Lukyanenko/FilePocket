using FilePocket.Client.Services.Files.Models;
using FilePocket.Client.Services.Files.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages
{
    public partial class File
    {
        [Parameter]
        public string PocketIdParam { get; set; } = string.Empty;

        [Parameter]
        public string FileIdParam { get; set; } = string.Empty;

        [Inject] 
        private IFileRequests FileRequests { get; set; } = default!;

        private Guid _pocketId;
        private Guid _fileId;

        private FileModel? _file;
        private string _imageContent = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _pocketId = Guid.Parse(PocketIdParam);
            _fileId = Guid.Parse(FileIdParam);

            _file = await FileRequests.GetFileAsync(_pocketId, _fileId);

            var base64Image = Convert.ToBase64String(_file.FileByteArray!);
            _imageContent = $"data:image/jpeg;base64,{base64Image}";
        }
    }
}

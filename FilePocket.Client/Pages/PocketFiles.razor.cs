using FilePocket.Client.Services.Files.Models;
using FilePocket.Client.Services.Files.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages
{
    public partial class PocketFiles
    {
        [Parameter]
        public string PocketIdParam { get; set; } = string.Empty;

        [Inject] private IFileRequests FileRequests { get; set; } = default!;

        private Guid _pocketId => Guid.Parse(PocketIdParam);

        private List<FileInfoModel> _files = new List<FileInfoModel>();

        protected override async Task OnInitializedAsync()
        {
           _files = await FileRequests.GetFilesAsync(_pocketId);
        }
    }
}

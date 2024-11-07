using FilePocket.Client.Services.Pockets.Models;
using FilePocket.Client.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages
{
    public partial class CreateFolder
    {
        [Parameter]
        public string PocketIdParam { get; set; } = string.Empty;

        private string _folderName = string.Empty;
        private bool _validName = true;

        [Inject] private IPocketRequests FolderRequests { get; set; } = default!;

        private async Task CreateFolderAsync()
        {
           
        }

        private void NameChanged()
        {
            _validName = !string.IsNullOrEmpty(_folderName);
        }
    }
}

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
            if (string.IsNullOrEmpty(_folderName))
            {
                _validName = false;
            }
            
            var model = new CreatePocketModel()
            {
                Name = _folderName,
                UserId = Guid.Parse("a9b78973-6458-498f-a313-ae26e56d223c")
            };

            var result = await FolderRequests.CreateAsync(model);

            if (result)
            {
                Navigation.NavigateTo($"/pockets/{PocketIdParam}/files");
            }
        }

        private void NameChanged()
        {
            _validName = !string.IsNullOrEmpty(_folderName);
        }
    }
}

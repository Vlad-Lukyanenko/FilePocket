using FilePocket.Client.Features.Folders.Models;
using FilePocket.Client.Services.Folders.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages.Folders
{
    public partial class CreateFolder
    {
        [Inject] private IFolderRequests FolderRequests { get; set; } = default!;

        [Parameter]
        public string PocketIdParam { get; set; } = string.Empty;

        [Parameter]
        public string FolderIdParam { get; set; } = string.Empty;

        private string _folderName = string.Empty;
        private bool _validName = true;
        private Guid _pocketId => Guid.Parse(PocketIdParam);

        private async Task CreateFolderAsync()
        {
            Guid? folderId = null;

            if(!string.IsNullOrWhiteSpace(FolderIdParam) && FolderIdParam != Guid.Empty.ToString())
            {
                folderId = Guid.Parse(FolderIdParam);
            }

            var folder = new FolderModel()
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = _folderName,
                ParentFolderId = folderId,
                PocketId = _pocketId
            };

            var result = await FolderRequests.CreateAsync(folder);

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

using FilePocket.Client.Features.Folders.Models;
using FilePocket.Client.Services.Folders.Requests;
using FilePocket.DataAccess;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FilePocket.Client.Pages.Folders
{
    public partial class CreateFolder
    {
        [Inject] 
        private IFolderRequests FolderRequests { get; set; } = default!;
      
        [Parameter]
        public string PocketIdParam { get; set; } = string.Empty;

        [Parameter]
        public string FolderIdParam { get; set; } = string.Empty;

        private string _folderName = string.Empty;
        private bool _validName = true;
        private bool _isDuplicate = false;
        private string GetGoBackUrl()
        {
            if (string.IsNullOrWhiteSpace(PocketIdParam) && string.IsNullOrWhiteSpace(FolderIdParam))
            {
                return $"/files";
            }

            var folderId = Guid.Parse(FolderIdParam);

            if (string.IsNullOrWhiteSpace(PocketIdParam) && !string.IsNullOrWhiteSpace(FolderIdParam))
            {
                return $"/folders/{folderId}/files";
            }

            if (!string.IsNullOrWhiteSpace(PocketIdParam) && string.IsNullOrWhiteSpace(FolderIdParam))
            {
                return $"/pockets/{PocketIdParam}/files";
            }

            return $"/pockets/{PocketIdParam}/folders/{folderId}/files";
        }

        private async Task CreateFolderAsync()
        {
            Guid? pocketId = null;
            if(!string.IsNullOrWhiteSpace(PocketIdParam) && PocketIdParam != Guid.Empty.ToString())
            {
                pocketId = Guid.Parse(PocketIdParam);
            }
            Guid? folderId = null;
            if(!string.IsNullOrWhiteSpace(FolderIdParam) && FolderIdParam != Guid.Empty.ToString())
            {
                folderId = Guid.Parse(FolderIdParam);
            }
            var isDuplicate = await FolderRequests.FolderExistsAsync(_folderName, pocketId, folderId);
            if (isDuplicate)
            {
                _isDuplicate = true;
                return;
            }

            var folder = new FolderModel()
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = _folderName,
                ParentFolderId = folderId,
                PocketId = pocketId
            };

            var result = await FolderRequests.CreateAsync(folder);

            if (result)
            {
                var url = GetGoBackUrl();
                Navigation.NavigateTo(url);
            }
        }

        private void NameChanged()
        {
            _validName = !string.IsNullOrEmpty(_folderName);
        }
        
    }
}

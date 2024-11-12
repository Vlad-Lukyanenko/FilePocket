using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages.Files
{
    public partial class FolderFiles
    {
        [Parameter]
        public string PocketIdParam { get; set; } = string.Empty;

        [Parameter]
        public string FolderIdParam { get; set; } = string.Empty;

        public Guid _pocketId => Guid.Parse(PocketIdParam);

        public Guid _folderId => Guid.Parse(FolderIdParam);
    }
}

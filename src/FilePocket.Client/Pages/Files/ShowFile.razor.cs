using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages.Files
{
    public partial class ShowFile
    {
        [Parameter]
        public string PocketId { get; set; } = string.Empty;
        
        [Parameter]
        public string? FolderId { get; set; } = null;

        [Parameter]
        public string FileId{ get; set; } = string.Empty;
    }
}

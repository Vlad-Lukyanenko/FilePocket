using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages.Files
{
    public partial class PocketFiles
    {
        [Parameter]
        public string PocketIdParam { get; set; } = string.Empty;

        private Guid _pocketId => Guid.Parse(PocketIdParam);
    }
}

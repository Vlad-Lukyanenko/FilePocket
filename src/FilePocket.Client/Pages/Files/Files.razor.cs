using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages.Files
{
    public partial class Files
    {
        [Parameter]
        public string? PocketId { get; set; } = string.Empty;

        [Parameter]
        public string? FolderId { get; set; } = null;

        public Guid? _pocketId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PocketId))
                {
                    return null;
                }

                return Guid.Parse(PocketId);
            }
        }

        public Guid? _folderId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FolderId))
                {
                    return null;
                }

                return Guid.Parse(FolderId);
            }
        }
    }
}

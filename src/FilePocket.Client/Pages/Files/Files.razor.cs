using FilePocket.Client.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages.Files
{
    public partial class Files
    {
        [Parameter]
        public string PocketId { get; set; } = string.Empty;

        [Parameter]
        public string? FolderId { get; set; } = null;

        [Inject] 
        private IPocketRequests PocketRequests { get; set; } = default!;

        private Guid _pocketId;

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

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrWhiteSpace(PocketId))
            {
                var defaultPocket = await PocketRequests.GetDefaultAsync();

                _pocketId = defaultPocket.Id;
            }
            else
            {
                _pocketId = Guid.Parse(PocketId);
            }
        }
    }
}

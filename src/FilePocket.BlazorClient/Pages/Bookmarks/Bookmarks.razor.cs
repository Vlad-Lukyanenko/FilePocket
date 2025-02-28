using FilePocket.BlazorClient.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.Pages.Bookmarks;

public partial class Bookmarks
{
    [Parameter] public Guid PocketId { get; set; }
    [Parameter] public Guid? FolderId { get; set; }

    [Inject] private IPocketRequests PocketRequests { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (PocketId == Guid.Empty)
        {
            var defaultPocket = await PocketRequests.GetDefaultAsync();
            PocketId = defaultPocket.Id;
        }
    }

    private string GetCreateBookmarkUrl()
    {
        if (FolderId is null)
        {
            return $"/pockets/{PocketId}/bookmarks/new";
        }

        return $"/pockets/{PocketId}/folders/{FolderId}/bookmarks/new";
    }
}

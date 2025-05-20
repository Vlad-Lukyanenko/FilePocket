using FilePocket.BlazorClient.Features.Trash;
using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.Pages;

public partial class Trash
{
    private bool _clearTrashStarted;

    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ITrashRequests TrashRequests { get; set; } = default!;

    private async Task ClearTrashClickAsync()
    {
        await TrashRequests.ClearAllTrashAsync();

        Navigation.NavigateTo("/");
    }

    private void Navigate(string uri)
    {
        Navigation.NavigateTo(uri);
    }
}

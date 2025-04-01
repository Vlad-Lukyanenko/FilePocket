using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.Pages;

public partial class Trash
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private void Navigate(string uri)
    {
        Navigation.NavigateTo(uri);
    }
}

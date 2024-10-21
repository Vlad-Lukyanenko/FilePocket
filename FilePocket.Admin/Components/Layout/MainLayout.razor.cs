using FilePocket.Admin.AuthFeatures;
using Microsoft.AspNetCore.Components;

namespace FilePocket.Admin.Components.Layout;

public partial class MainLayout
{
    private bool sidebarExpanded = true;

    [Inject] AuthStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] NavigationManager NavigationManager { get; set; } = default!;

    private async Task Logout()
    {
        await AuthStateProvider.LogoutAsync();
        NavigationManager.NavigateTo("/", true);
    }
}

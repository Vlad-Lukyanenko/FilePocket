using FilePocket.BlazorClient.Features.Profiles.Models;
using FilePocket.BlazorClient.Features.Profiles.Requests;
using FilePocket.BlazorClient.Features.Users.Models;
using FilePocket.BlazorClient.Features.Users.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;

namespace FilePocket.BlazorClient.Pages;

public partial class Profile : ComponentBase
{
    private ProfileModel _profile = new();
    private bool _isLoading = true;
    private string _alertMessage = "";
    private string _alertStyle = "display: none;";

    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] private IUserRequests UserRequests { get; set; } = default!;
    [Inject] private IProfileRequests ProfileRequests { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userAuth = authState.User;
        var userStringId = userAuth.FindFirst(c => c.Type == "uid")?.Value;
        var userId = new Guid(userStringId!);

        if (userId != Guid.Empty)
        {
            _profile = await ProfileRequests.GetByUserIdAsync(userId);
        }

        _isLoading = false;
    }

    private async Task SaveChangesAsync(MouseEventArgs e)
    {
        if (string.IsNullOrEmpty(_profile.FirstName) || string.IsNullOrEmpty(_profile.LastName))
        {
            return;
        }

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        var request = new UpdateUserRequest
        {
            UserName = authState.User.Identity?.Name!,
            FirstName = _profile.FirstName,
            LastName = _profile.LastName
        };

        await UserRequests.UpdateUserAsync(request);

        var profileToUpdate = new UpdateProfileModel
        {
            Id = _profile.Id,
            FirstName = _profile.FirstName,
            LastName = _profile.LastName,
            IconId = _profile.IconId
        };

        var isUpdated = await ProfileRequests.UpdateAsync(profileToUpdate);

        if (isUpdated)
        {
            await TriggerNotification();
        }
    }

    private string GetGoBackUrl()
    {
        return string.Empty;
    }

    private async Task TriggerNotification()
    {
        await ShowAlert("Updates successfully saved");
        await Task.Delay(3000);
        await CloseAlert();
    }

    private async Task ShowAlert(string message)
    {
        _alertMessage = message;
        _alertStyle = "display: block;";

        await InvokeAsync(StateHasChanged);
    }

    private async Task CloseAlert()
    {
        _alertMessage = "";
        _alertStyle = "display: none;";

        await InvokeAsync(StateHasChanged);
    }
}

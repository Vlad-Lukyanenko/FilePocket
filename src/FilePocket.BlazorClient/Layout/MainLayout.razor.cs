using FilePocket.BlazorClient.Features.Users.Models;
using FilePocket.BlazorClient.Features.Users.Requests;
using FilePocket.BlazorClient.Helpers;
using FilePocket.BlazorClient.Services.Files.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace FilePocket.BlazorClient.Layout;

public partial class MainLayout : IDisposable
{
    private string? _iconName;
    LoggedInUserModel? _user;
    private bool _menuOpen;
    private string? _icon;
    private bool _render;

    [Inject] AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] IUserRequests UserRequests { get; set; } = default!;
    [Inject] private IFileRequests FileRequests { get; set; } = default!;
    [Inject] private StateContainer<LoggedInUserModel> UserStateContainer { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userName = authState.User.Identity?.Name!;

        _user = await UserRequests.GetByUserNameAsync(userName);

        if (_user is not null)
        {
            var fisrtName = string.IsNullOrEmpty(_user.FirstName) ? string.Empty : _user.FirstName.AsSpan(0, 1);
            var lastName = string.IsNullOrEmpty(_user.LastName) ? string.Empty : _user.LastName.AsSpan(0, 1);
            _iconName = string.Concat(fisrtName, lastName).ToUpper();

            if (_iconName.Length == 0)
            {
                _iconName = _user!.UserName![..1].ToUpper();
            }

            if (_user.Profile!.IconId is not null && _user.Profile!.IconId != Guid.Empty)
            {
                var avatar = await FileRequests.GetImageThumbnailAsync((Guid)_user.Profile.IconId, 500);
                _icon = Convert.ToBase64String(avatar.FileByteArray!);
            }
        }

        Navigation.LocationChanged += OnLocationChanged;
        UserStateContainer.OnStateChange += async () => await UpdateUserStateAsync();
        NavigationHistory.AddToHistory(Navigation.Uri);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        _menuOpen = false;
        NavigationHistory.AddToHistory(e.Location);
    }

    public void Dispose()
    {
        Navigation.LocationChanged -= OnLocationChanged;
        UserStateContainer.OnStateChange -= async () => await UpdateUserStateAsync();
        GC.SuppressFinalize(this);
    }

    private string IsActive(string href)
    {
        return Navigation.Uri.EndsWith(href, StringComparison.OrdinalIgnoreCase) ? "active" : "";
    }

    private async Task CloseOffcanvasAndNavigate(string url)
    {
        await JS.InvokeVoidAsync("closeOffcanvasMenu");
        Navigation.NavigateTo(url);
    }

    private void SwitchUserInfoDialog()
    {
        _menuOpen = !_menuOpen;
    }

    private string GetDisplayedName()
    {
        if (_user is null || (string.IsNullOrEmpty(_user.FirstName) && string.IsNullOrEmpty(_user.LastName)))
        {
            return string.Empty;
        }

        if (!string.IsNullOrEmpty(_user.FirstName) && !string.IsNullOrEmpty(_user.LastName))
        {
            return $"{_user.FirstName} {_user.LastName}";
        }

        return string.IsNullOrEmpty(_user.FirstName) ? _user.LastName! : _user.FirstName;
    }

    private async Task UpdateUserStateAsync()
    {
        _user = UserStateContainer.Value;

        if (_user!.Profile!.IconId is not null && _user.Profile!.IconId != Guid.Empty)
        {
            var avatar = await FileRequests.GetImageThumbnailAsync((Guid)_user.Profile.IconId, 500);
            _icon = Convert.ToBase64String(avatar.FileByteArray!);
        }

        await InvokeAsync(StateHasChanged);
    }
}


using FilePocket.BlazorClient.Features.Storage.Models;
using FilePocket.BlazorClient.Features.Storage.Requests;
using FilePocket.BlazorClient.Features.Users.Models;
using FilePocket.BlazorClient.Features.Users.Requests;
using FilePocket.BlazorClient.Helpers;
using FilePocket.BlazorClient.Services.Files.Models;
using FilePocket.BlazorClient.Services.Files.Requests;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using FilePocket.BlazorClient.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace FilePocket.BlazorClient.Layout;

public partial class MobileMenu : IDisposable
{
    private string? _iconName;
    private LoggedInUserModel? _user;
    private bool _menuOpen;
    private string? _icon;
    private bool _render;
    private bool _isFilesMenuOpen = false;

    public Dictionary<FileTypes, double> _occupiedSpaceByFileType = new();

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private IUserRequests UserRequests { get; set; } = default!;
    [Inject] private IFileRequests FileRequests { get; set; } = default!;
    [Inject] private IStorageRequests StorageRequests { get; set; } = default!;
    [Inject] private StateContainer<LoggedInUserModel> UserStateContainer { get; set; } = default!;
    [Inject] private StateContainer<StorageConsumptionModel> StorageStateContainer { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IPocketRequests PocketRequests { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userName = authState.User.Identity?.Name!;

        _user = await UserRequests.GetByUserNameAsync(userName);

        if (_user is not null)
        {
            StateHasChanged();

            var firstName = string.IsNullOrEmpty(_user.FirstName) ? string.Empty : _user.FirstName.Substring(0, 1);
            var lastName = string.IsNullOrEmpty(_user.LastName) ? string.Empty : _user.LastName.Substring(0, 1);
            _iconName = string.Concat(firstName, lastName).ToUpper();

            if (string.IsNullOrEmpty(_iconName))
            {
                _iconName = _user.UserName![..1].ToUpper();
            }

            if (_user.Profile?.IconId is not null && _user.Profile.IconId != Guid.Empty)
            {
                var avatar = await FileRequests.GetImageThumbnailAsync((Guid)_user.Profile.IconId, 500);
                _icon = Convert.ToBase64String(avatar.FileByteArray!);
            }
        }

        Navigation.LocationChanged += OnLocationChanged;
        UserStateContainer.OnStateChange += async () => await UpdateUserStateAsync();
        StorageStateContainer.OnStateChange += async () => await UpdateStorageStateAsync();
        // NavigationHistory.Instance.AddToHistory(Navigation.Uri);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        _menuOpen = false;
        // NavigationHistory.AddToHistory(e.Location); 
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
        await JSRuntime.InvokeVoidAsync("closeOffcanvasMenu");
        await Task.Delay(300);
        Navigation.NavigateTo(url);
    }

    private void NavigateToProfile()
    {
        Navigation.NavigateTo("/profile", forceLoad: false);
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

        if (_user?.Profile?.IconId is not null && _user.Profile.IconId != Guid.Empty)
        {
            var avatar = await FileRequests.GetImageThumbnailAsync((Guid)_user.Profile.IconId, 500);
            _icon = Convert.ToBase64String(avatar.FileByteArray!);
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateStorageStateAsync()
    {
        await Task.CompletedTask;
    }

    private void ToggleFilesMenu()
    {
        _isFilesMenuOpen = !_isFilesMenuOpen;
        StateHasChanged();
    }
}

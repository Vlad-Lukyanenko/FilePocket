using FilePocket.BlazorClient.Features.Storage.Models;
using FilePocket.BlazorClient.Features.Storage.Requests;
using FilePocket.BlazorClient.Features.Users.Models;
using FilePocket.BlazorClient.Features.Users.Requests;
using FilePocket.BlazorClient.Services.Files.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace FilePocket.BlazorClient.Layout;

public partial class MainLayout : IDisposable
{
    private string? _iconName;
    LoggedInUserModel? _user;
    private bool _menuOpen;
    private string? _icon;

    [Inject] AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] IUserRequests UserRequests { get; set; } = default!;
    [Inject] private IFileRequests FileRequests { get; set; } = default!;
    [Inject] private IStorageRequests StorageRequests { get; set; } = default!;

    private StorageConsumptionModel _storageConsumption = new();

    private string _unoccupiedStorageSpace = "0";
    private string _storageCapacity = "0";
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userName = authState.User.Identity?.Name!;

        _user = await UserRequests.GetByUserNameAsync(userName);

        if (_user is not null)
        {
            _user.FirstName ??= string.Empty;
            _user.LastName ??= string.Empty;
            _storageConsumption = await StorageRequests.GetStorageConsumption();
            GetUnoccupiedStorageSpace();
            GetStorageCapacity();
            StateHasChanged();
            _iconName = string.Concat(_user.FirstName.AsSpan(0, 1), _user.LastName.AsSpan(0, 1)).ToUpper();


            if (_iconName.Length == 0)
            {
                _iconName = _user!.UserName![..1].ToUpper();
            }

            if (_user.Profile!.IconId is not null && _user.Profile!.IconId != Guid.Empty)
            {
                var avatar = await FileRequests.GetImageThumbnailAsync((Guid)_user.Profile!.IconId, 500);
                _icon = Convert.ToBase64String(avatar.FileByteArray!);
            }
        }

        Navigation.LocationChanged += OnLocationChanged;
        NavigationHistory.AddToHistory(Navigation.Uri);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        _menuOpen = false;
        NavigationHistory.AddToHistory(e.Location);
    }

    protected override void OnParametersSet()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        Navigation.LocationChanged -= OnLocationChanged;
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
        if (_user == null) return string.Empty;

        if (_user.FirstName!.Length > 0 && _user.LastName!.Length > 0)
        {
            return $"{_user.FirstName} {_user.LastName}";
        }

        return _user.FirstName!.Length > 0 ? _user.FirstName! : _user.LastName!;
    }
    private void GetUnoccupiedStorageSpace()
    {
        _unoccupiedStorageSpace = Math.Round(((1 - (_storageConsumption.Used / _storageConsumption.Total)) * 100), 1)
            .ToString()
            .Replace(',', '.');
    }

    private void GetStorageCapacity()
    {
        _storageCapacity = Math.Round((_storageConsumption.Total / 1000), 1).ToString();
    }
}


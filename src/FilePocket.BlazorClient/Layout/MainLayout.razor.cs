
using FilePocket.BlazorClient.Features.Users.Models;

using FilePocket.BlazorClient.Features.Users.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace FilePocket.BlazorClient.Layout
{
    public partial class MainLayout : IDisposable
    {
        private string? _iconName;
        LoggedInUserModel? _user;
        private bool _menuOpen;

        [Inject] AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] IUserRequests UserRequests { get; set; } = default!;

        protected override async Task OnParametersSetAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var userName = authState.User.Identity?.Name!;

            _user = await UserRequests.GetByUserNameAsync(userName);

            if (_user is not null)
            {
                _user.FirstName ??= string.Empty;
                _user.LastName ??= string.Empty;
                _iconName = string.Concat(_user.FirstName.AsSpan(0, 1), _user.LastName.AsSpan(0, 1)).ToUpper();

                if (_iconName.Length == 0)
                {
                    _iconName = _user!.UserName![..1].ToUpper();
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
    }


    
}


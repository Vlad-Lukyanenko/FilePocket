using FilePocket.BlazorClient.Features.Users.Requests;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using FilePocket.BlazorClient.Features.Users.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace FilePocket.BlazorClient.Pages
{
    public partial class Profile : ComponentBase
    {
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        
        [Inject] 
        private IUserRequests UserRequests { get; set; } = default!;

        private string? _firstName = string.Empty;
        private string? _lastName = string.Empty;
        private string _userName = string.Empty;

        private bool _isLoading = true;
        private string _alertMessage = "";
        private string _alertStyle = "display: none;";

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            _userName = authState.User.Identity?.Name!;

            var user = await UserRequests.GetByUserNameAsync(_userName);

            if (user is not null)
            {
                _firstName = user.FirstName;
                _lastName = user.LastName;
            }

            _isLoading = false;
        }

        private async Task SaveChangesAsync(MouseEventArgs e)
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            var request = new UpdateUserRequest
            {
                UserName = authState.User.Identity?.Name!,
                FirstName = _firstName,
                LastName = _lastName
            };

            await UserRequests.UpdateUserAsync(request);

            await TriggerNotification();
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
}

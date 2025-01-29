using FilePocket.Client.Services.Authentication.Models;
using FilePocket.Client.Services.Authentication.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FilePocket.Client.Pages
{
    public partial class SignIn
    {
        [Inject] 
        NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IAuthentictionRequests AuthRequests { get; set; } = default!;

        private LoginModel _loginModel = new LoginModel();

        private async void FormSubmitted(EditContext editContext)
        {
            var success = await AuthRequests.LoginAsync(_loginModel);

            if (success.IsSuccess)
            {
                Navigation.NavigateTo("/");
            }
        }
    }
}

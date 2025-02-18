using FilePocket.BlazorClient.Services.Authentication.Models;
using FilePocket.BlazorClient.Services.Authentication.Requests;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.MyComponents
{
    public partial class SignInComponent
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

using FilePocket.BlazorClient.Services.Authentication.Models;
using FilePocket.BlazorClient.Services.Authentication.Requests;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.MyComponents
{
    public partial class SignInComponent
    {
        private bool showEmailError = false;
        private bool showPasswordError = false;
        private bool isPasswordVisible = false;
        private string passwordInputType = "password";
        private string passwordIcon = "bi bi-eye";

        [Inject]
        NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IAuthentictionRequests AuthRequests { get; set; } = default!;

        private LoginModel _loginModel = new LoginModel();

        private bool _wrongEmailOrPassword = false;

        private int _numberOfTimerStarts = 0;

        private void ValidateEmail()
        {
            showEmailError = string.IsNullOrWhiteSpace(_loginModel.Email);
        }

        private void ValidatePassword()
        {
            showPasswordError = string.IsNullOrWhiteSpace(_loginModel.Password);
        }

        private void TogglePassword()
        {
            isPasswordVisible = !isPasswordVisible;
            passwordInputType = isPasswordVisible ? "text" : "password";
            passwordIcon = isPasswordVisible ? "bi bi-eye-slash" : "bi bi-eye";
        }
        
        private async void FormSubmitted(EditContext editContext)
        {
            var success = await AuthRequests.LoginAsync(_loginModel);

            if (success.IsSuccess)
            {
                Navigation.NavigateTo("/");
            }
            OpenErrorLine();
        }

        private async void OpenErrorLine()
        {
            if (_wrongEmailOrPassword == false)
            {
                _wrongEmailOrPassword = true;
                StateHasChanged();
            }

            _numberOfTimerStarts++;
            await Task.Delay(5000);
            _numberOfTimerStarts--;

            if (_numberOfTimerStarts == 0 && _wrongEmailOrPassword == true)
            {
                CloseErrorLine();
            }
        }

        private void CloseErrorLine()
        {
            _wrongEmailOrPassword = false;
            StateHasChanged();
        }

    }
}

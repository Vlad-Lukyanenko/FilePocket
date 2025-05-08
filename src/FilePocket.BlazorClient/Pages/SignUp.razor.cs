using FilePocket.BlazorClient.Services.Authentication.Models;
using FilePocket.BlazorClient.Services.Authentication.Requests;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace FilePocket.BlazorClient.Pages
{
    public partial class SignUp
    {
        private bool showEmailError = false;
        private bool showPasswordError = false;
        private bool showConfirmPasswordError = false;
        private bool isPasswordVisible = false;
        private bool isConfirmPasswordVisible = false;
        private string passwordInputType = "password";
        private string confirmPasswordInputType = "password";
        private string passwordIcon = "bi bi-eye";
        private string confirmPasswordIcon = "bi bi-eye";

        [Inject]
        NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IAuthentictionRequests AuthRequests { get; set; } = default!;

        private RegistrationRequest _registrationRequest = new RegistrationRequest();

        private List<string> _errors = new List<string>();


        private void ValidateEmail()
        {
            showEmailError = string.IsNullOrWhiteSpace(_registrationRequest.Email);
        }

        private void ValidatePassword()
        {
            showPasswordError = string.IsNullOrWhiteSpace(_registrationRequest.Password);
        }

        private void ValidateConfirmPassword()
        {
            showConfirmPasswordError = string.IsNullOrWhiteSpace(_registrationRequest.ConfirmPassword);
        }

        private void TogglePassword()
        {
            isPasswordVisible = !isPasswordVisible;
            passwordInputType = isPasswordVisible ? "text" : "password";
            passwordIcon = isPasswordVisible ? "bi bi-eye-slash" : "bi bi-eye";
        }

        private void ToggleConfirmPassword()
        {
            isConfirmPasswordVisible = !isConfirmPasswordVisible;
            confirmPasswordInputType = isConfirmPasswordVisible ? "text" : "password";
            confirmPasswordIcon = isConfirmPasswordVisible ? "bi bi-eye-slash" : "bi bi-eye";
        }

        private async void HandleValidSubmit()
        {
            var response = await AuthRequests.RegisterUserAsync(_registrationRequest);

            var content = await response.Content.ReadAsStringAsync();

            if (content.Length > 0)
            {
                try
                {
                    var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();

                    if (errors != null)
                    {
                        if (errors.ContainsKey("DuplicateEmail"))
                        {
                            _errors.Add("Oops! This email is already taken.");
                        }
                        else if (errors.ContainsKey("PasswordRequiresDigit"))
                        {
                            _errors.Add("Oops! Passwords must have at least one digit ('0'-'9').");
                        }
                    }

                }
                catch (Exception)
                {
                    //TODO: nothing
                }
            }

            StateHasChanged();

            _errors = new List<string>();

            if (response.IsSuccessStatusCode)
            {
                Navigation.NavigateTo("/sign-in");
            }
        }
    }
}

using FilePocket.BlazorClient.Services.Authentication.Models;
using FilePocket.BlazorClient.Services.Authentication.Requests;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FilePocket.BlazorClient.Pages
{
    public partial class SignUp
    {
        [Inject]
        NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IAuthentictionRequests AuthRequests { get; set; } = default!;

        private RegistrationRequest _registrationRequest = new RegistrationRequest();

        private List<string> _errors = new List<string>();

        private async void HandleValidSubmit()
        {
            var response = await AuthRequests.RegisterUserAsync(_registrationRequest);

            var content = await response.Content.ReadAsStringAsync();

            if (content.Length > 0)
            {
                try
                {
                    var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();

                    if (errors != null && errors.ContainsKey("DuplicateEmail"))
                    {
                        _errors.Add("Oops! This email is already taken.");
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

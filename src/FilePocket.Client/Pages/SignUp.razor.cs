using FilePocket.Client.Services.Authentication.Models;
using FilePocket.Client.Services.Authentication.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FilePocket.Client.Pages
{
    public partial class SignUp
    {
        [Inject] 
        NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IAuthentictionRequests AuthRequests { get; set; } = default!;

        private RegistrationRequest _registrationRequest = new RegistrationRequest();

        private async void FormSubmitted(EditContext editContext)
        {
            var success = await AuthRequests.RegisterUserAsync(_registrationRequest);

            if (success)
            {
                Navigation.NavigateTo("/sign-in");
            }
        }
    }
}

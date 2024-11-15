using FilePocket.Client.Services.Authentication.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages
{
    public partial class Logout
    {
        [Inject] 
        NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IAuthentictionRequests AuthRequests { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        { 
            await AuthRequests.Logout();
            Navigation.NavigateTo("/");
        }
    }
}

using FilePocket.BlazorClient.Services.Authentication.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.Pages
{
    public partial class Logout : ComponentBase
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

using FilePocket.BlazorClient.Features.Notes.Requests;
using FilePocket.BlazorClient.Features.Users.Requests;
using FilePocket.BlazorClient.Services.Files.Requests;
using FilePocket.BlazorClient.Services.Folders.Requests;
using FilePocket.BlazorClient.Services.Pockets.Models;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FilePocket.BlazorClient.Pages.Notes
{
    public partial class Notes
    {
        [Inject]
        AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        [Inject]
        IUserRequests UserRequests { get; set; } = default!;

        [Inject]
        private INoteRequests NoteRequests { get; set; } = default!;

        private Guid _userId = Guid.Empty;

        protected override async Task OnParametersSetAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var userName = authState.User.Identity?.Name!;

            var user = await UserRequests.GetByUserNameAsync(userName);
            if (user == null)
            {
                return;
            }

            _userId = user.Id!.Value;
        }
    }
}

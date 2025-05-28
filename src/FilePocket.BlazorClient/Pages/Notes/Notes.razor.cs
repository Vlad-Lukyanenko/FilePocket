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
        [Parameter]
        public string? FolderId { get; set; }

        [Parameter]
        public string PocketId { get; set; } = string.Empty;

        [Inject]
        AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        [Inject]
        IUserRequests UserRequests { get; set; } = default!;

        [Inject]
        IFolderRequests FolderRequests { get; set; } = default!;    

        [Inject]
        private INoteRequests NoteRequests { get; set; } = default!;

        private Guid _userId = Guid.Empty;

        private Guid? _pocketId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PocketId))
                {
                    return null;
                }

                return Guid.Parse(PocketId);
            }
        }

        private Guid? _folderId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FolderId))
                {
                    return null;
                }

                return Guid.Parse(FolderId);
            }
        }

        protected override async Task OnInitializedAsync()
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

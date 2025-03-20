using FilePocket.BlazorClient.Features.Users.Requests;
using FilePocket.BlazorClient.MyComponents;
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
        private const string DefaultNotesPocketName = "MyNotes";
        private Guid? _pocketId;
        private Guid? _folderId;

        [Inject]
        AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        [Inject]
        IUserRequests UserRequests { get; set; } = default!;

        [Inject]
        private IFileRequests FileRequests { get; set; } = default!;

        [Inject]
        private IFolderRequests FolderRequests { get; set; } = default!;

        [Inject]
        private IPocketRequests PocketRequests { get; set; } = default!;

        protected override async Task OnParametersSetAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var userName = authState.User.Identity?.Name!;

            var user = await UserRequests.GetByUserNameAsync(userName);
            if (user == null)
            {
                return;
            }

            var pockets = await PocketRequests.GetAllCustomAsync();
            var notesPocket = pockets.FirstOrDefault(p => p.Name == DefaultNotesPocketName);
            if (notesPocket == null)
            {
                var newPocket = new CreatePocketModel
                {
                    Name = DefaultNotesPocketName,
                    UserId = user.Id!.Value,
                    Description = "Default place to store my notes"
                };

                var result = await PocketRequests.CreateAsync(newPocket);
                if (!result)
                {
                    return;
                }

                notesPocket = (await PocketRequests.GetAllCustomAsync())
                    .FirstOrDefault(p => p.Name == DefaultNotesPocketName);
            }

            _pocketId = notesPocket?.Id;
        }
    }
}

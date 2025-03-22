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
        private string? _defaultNotesPocketName;
        private Guid? _pocketId;
        private Guid? _folderId;

        [Inject]
        AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        [Inject]
        IUserRequests UserRequests { get; set; } = default!;

        [Inject]
        IConfiguration Configuration { get; set; } = default!;

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

            _defaultNotesPocketName = Configuration.GetSection("DefaultNotesPocketName")?.Value;
            if (_defaultNotesPocketName == null)
            {
                return;
            }

            var pockets = await PocketRequests.GetAllCustomAsync();
            var notesPocket = pockets.FirstOrDefault(p => p.Name == _defaultNotesPocketName);
            if (notesPocket == null)
            {
                var newPocket = new CreatePocketModel
                {
                    Name = _defaultNotesPocketName,
                    UserId = user.Id!.Value,
                    Description = "Default place to store my notes"
                };

                var result = await PocketRequests.CreateAsync(newPocket);
                if (!result)
                {
                    return;
                }

                notesPocket = (await PocketRequests.GetAllCustomAsync())
                    .FirstOrDefault(p => p.Name == _defaultNotesPocketName);
            }

            _pocketId = notesPocket?.Id;
        }
    }
}

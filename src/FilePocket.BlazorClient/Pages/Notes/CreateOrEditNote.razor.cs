using FilePocket.BlazorClient.Features.Notes.Requests;
using FilePocket.BlazorClient.Features.Users.Requests;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using FilePocket.BlazorClient.Features.Notes.Models;
using Microsoft.AspNetCore.Components.Web;

namespace FilePocket.BlazorClient.Pages.Notes
{
    public partial class CreateOrEditNote
    {
        [Parameter]
        public Guid Id { get; set; }

        [Inject]
        private INoteRequests NoteRequests { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        [Inject]
        private IUserRequests UserRequests { get; set; } = default!;

        private NoteModel? _note;
        private Guid _userId = Guid.Empty;
        private bool _editTitle;

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

            if (Id != Guid.Empty)
            {
                _note = await NoteRequests.GetByIdAsync(Id);
            }
            else
            {
                _note = new()
                {
                    UserId = _userId,
                    Title = "New Note"
                };
            }

            await base.OnParametersSetAsync();
        }

        private async Task SaveNote()
        {
            var result = await NoteRequests.CreateAsync(new NoteCreateModel
            {
                UserId = _note.UserId,
                Title = _note!.Title,
                Content = _note.Content
            });

            if (result.Id != Guid.Empty)
            {
                _note.Id = result.Id;
                _note.CreatedAt = result.CreatedAt;
                _note.UpdatedAt = result.UpdatedAt;
            }
        }

        private async Task UpdateNote()
        {
            var result = await NoteRequests.UpdateAsync(_note!);

            if (result.UpdatedAt != default)
            {
                _note!.UpdatedAt = result.UpdatedAt;
            }
        }

        private async Task ChangeNoteContent(string content)
        {
            _note!.Content = content;
            await SaveOrUpdateNote();
        }

        private async Task SaveOrUpdateNote()
        {
            if (_note == null)
            {
                return;
            }

            if (_note.Id == Guid.Empty)
            {
                await SaveNote();
            }
            else
            {
                await UpdateNote();
            }
        }

        private void CloseEditor()
        {
            Navigation.NavigateTo("/notes");
        }

        private void EditTile()
        {
            _editTitle = true;
        }

        private async Task SaveTitle()
        {
            _editTitle = false;
            await SaveOrUpdateNote();
        }

        private async Task InvokeSaveTitle(KeyboardEventArgs e)
        {
            if(e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await SaveTitle();
            }
        }
    }
}

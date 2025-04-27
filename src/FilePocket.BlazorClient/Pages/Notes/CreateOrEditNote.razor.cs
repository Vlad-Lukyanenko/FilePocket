using FilePocket.BlazorClient.Features.Notes.Requests;
using FilePocket.BlazorClient.Features.Users.Requests;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using FilePocket.BlazorClient.Features.Notes.Models;
using Microsoft.AspNetCore.Components.Web;
using FilePocket.BlazorClient.Features;

namespace FilePocket.BlazorClient.Pages.Notes
{
    public partial class CreateOrEditNote
    {
        [Parameter]
        public Guid Id { get; set; }

        [Parameter]
        public Guid PocketId { get; set; }

        [Parameter]
        public Guid? FolderId { get; set; }

        [Inject]
        private INoteRequests NoteRequests { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        [Inject]
        private IUserRequests UserRequests { get; set; } = default!;

        [Inject]
        private NavigationHistoryService NavigationHistory { get; set; } = default!;

        private NoteModel? _note;
        private Guid _userId = Guid.Empty;
        private bool _editTitle;
        private const string DateTimePlaceholder = "--.--.---- --:--:--";
        private string? _createdAt;
        private string? _updatedAt;

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

                _createdAt = _note.CreatedAt.ToString();
                _updatedAt = GetUpdatedDateValue(_note.UpdatedAt);
            }
            else
            {
                _note = new()
                {
                    UserId = _userId,
                    Title = "New Note",
                    FolderId = this.FolderId,
                    PocketId = this.PocketId
                };

                _createdAt = DateTimePlaceholder;
                _updatedAt = DateTimePlaceholder;
            }

            await base.OnParametersSetAsync();
        }

        private async Task SaveNote()
        {
            var result = await NoteRequests.CreateAsync(new NoteCreateModel
            {
                UserId = _note!.UserId,
                FolderId = _note.FolderId,
                PocketId = _note.PocketId,
                Title = _note!.Title,
                Content = _note.Content
            });

            if (result.Id != Guid.Empty)
            {
                _note.Id = result.Id;
                _note.CreatedAt = result.CreatedAt;
                _note.UpdatedAt = result?.UpdatedAt;

                _createdAt = _note.CreatedAt.ToString();
                _updatedAt = GetUpdatedDateValue(_note.UpdatedAt);
            }
        }

        private async Task UpdateNote()
        {
            var result = await NoteRequests.UpdateAsync(_note!);

            if (result.UpdatedAt != default)
            {
                _note!.UpdatedAt = result.UpdatedAt;
                _updatedAt = _note!.UpdatedAt.ToString();

                await InvokeAsync(StateHasChanged);
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
            var prevUrl = NavigationHistory.GetPreviousUrl() ?? "/notes";
            Navigation.NavigateTo(prevUrl);
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

        private void InvokeSaveTitle(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                _editTitle = false;
            }
        }

        private static string GetUpdatedDateValue(DateTime? updatedAt)
        {
            return (updatedAt == null ? DateTimePlaceholder : updatedAt.ToString())!;
        }
    }
}

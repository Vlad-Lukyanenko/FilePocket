using FilePocket.BlazorClient.Features.Notes.Models;
using FilePocket.BlazorClient.Features.Notes.Requests;
using FilePocket.BlazorClient.Services.Files.Models;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;

namespace FilePocket.BlazorClient.MyComponents
{
    public partial class NotesView
    {

        [Parameter]
        public Guid? UserId { get; set; }

        [Inject]
        private INoteRequests NoteRequests { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        private string _goBackUrl = string.Empty;

        private ObservableCollection<NoteModel>? _notes;

        private bool _firstRender = true;


        protected override async Task OnInitializedAsync()
        {
            await InitPage();
            StateHasChanged();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!_firstRender)
            {
                await OnInitializedAsync();
            }
            else
            {
                _firstRender = false;
            }
        }

        private async Task InitPage()
        {
            if (UserId == null)
            {
                return;
            }

            var notess = await NoteRequests.GetAllByUserId();

            _notes = [.. notess];
        }

        private void OpenEditor()
        {
            Navigation.NavigateTo("/notes/new");
        }
    }
}

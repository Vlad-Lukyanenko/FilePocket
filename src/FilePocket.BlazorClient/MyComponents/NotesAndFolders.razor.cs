using FilePocket.BlazorClient.Shared.Enums;
using FilePocket.BlazorClient.Features.Folders.Models;
using FilePocket.BlazorClient.Features.Notes.Models;
using FilePocket.BlazorClient.Features.Notes.Requests;
using FilePocket.BlazorClient.Services.Folders.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;
using FilePocket.BlazorClient.Features.Trash;
using FilePocket.BlazorClient.Services.Pockets.Requests;

namespace FilePocket.BlazorClient.MyComponents
{
    public partial class NotesAndFolders
    {

        [Parameter]
        public Guid? UserId { get; set; }

        [Parameter]
        public Guid? PocketId { get; set; }


        [Parameter]
        public Guid? FolderId { get; set; }

        [Inject]
        private INoteRequests NoteRequests { get; set; } = default!;

        [Inject]
        private IFolderRequests FolderRequests { get; set; } = default!;

        [Inject]
        private IPocketRequests PocketRequests { get; set; } = default!;

        [Inject]
        private ITrashRequests TrashRequests { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        private string _goBackUrl = string.Empty;
        private FolderModel? _currentFolder;
        private ObservableCollection<NoteModel>? _notes;
        private ObservableCollection<FolderModel>? _folders;
        private bool _removalProcessStarted;

        protected override async Task OnParametersSetAsync()
        {
            if (PocketId is null)
            {
                var defaultPocket = await PocketRequests.GetDefaultAsync();

                PocketId = defaultPocket.Id;
            }

            if (UserId == null)
            {
                return;
            }

            var folderTypes = new List<FolderType> { FolderType.Notes };

            if (FolderId == null)
            {
                _currentFolder = null;
                _folders = [.. (await FolderRequests.GetAllAsync(PocketId, folderTypes, false))];
            }
            else
            {
                _currentFolder = await FolderRequests.GetAsync(PocketId!.Value, FolderId.Value);
                _folders = [.. await FolderRequests.GetAllAsync(PocketId, FolderId.Value, folderTypes, false)];
            }

            _notes = [.. await NoteRequests.GetAllByUserIdAndFolderId(FolderId)];
            _goBackUrl = GetGoBackUrl();
        }

        private void OpenEditor()
        {
            var url = FolderId == null
                ? $"/pockets/{PocketId}/notes/new"
                : $"/pockets/{PocketId}/folders/{FolderId}/notes/new";

            Navigation.NavigateTo(url);
        }

        private async Task DeleteFolderClick()
        {
            if (FolderId is not null)
            {
                //MoveFolderToTrash does not exist
                //await TrashRequests.MoveFolderToTrash(FolderId.Value);
            }

            _removalProcessStarted = false;

            Navigation.NavigateTo(_goBackUrl);
        }

        private string GetGoBackUrl()
        {
            var pocketUrl = $"/pockets/{PocketId}";

            var parentFolderUrl = _currentFolder?.ParentFolderId is null
                ? ""
                : $"/folders/{_currentFolder!.ParentFolderId}";

            return $"{pocketUrl}{parentFolderUrl}/notes";
        }
    }
}

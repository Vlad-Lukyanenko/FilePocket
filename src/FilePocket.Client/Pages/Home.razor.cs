using FilePocket.Client.Features.Files.Models;
using FilePocket.Client.Features.SharedFiles.Models;
using FilePocket.Client.Features.SharedFiles.Requests;
using FilePocket.Client.Services.Files.Models;
using FilePocket.Client.Services.Files.Requests;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace FilePocket.Client.Pages
{
    public partial class Home
    {
        [Inject]
        private IFileRequests FileRequests { get; set; } = default!;

        [Inject]
        private ISharedFilesRequests SharedFilesRequests { get; set; } = default!;

        private List<SharedFileView>? _sharedFiles;

        private ObservableCollection<FileInfoModel> _files = new();
        
        protected override async Task OnInitializedAsync()
        {
            var files = await FileRequests.GetRecentFilesAsync();
            _files = new ObservableCollection<FileInfoModel>(files);

            _sharedFiles = await SharedFilesRequests.GetLatestAsync();
        }

        private string GetFileUrl(Guid fileId, Guid? pocketId, Guid? folderId)
        {
            if (pocketId is null && folderId is null)
            {
                return $"/files/{fileId}";
            }

            if (pocketId is null)
            {
                return $"/folders/{folderId}/files/{fileId}";
            }

            if (folderId is null)
            {
                return $"/pockets/{pocketId}/files/{fileId}";
            }

            return $"/pockets/{pocketId}/folders/{folderId}/files/{fileId}";
        }

        public static FileTypes ParseEnum(string value)
        {
            return (FileTypes)Enum.Parse(typeof(FileTypes), value, true);
        }
    }
}

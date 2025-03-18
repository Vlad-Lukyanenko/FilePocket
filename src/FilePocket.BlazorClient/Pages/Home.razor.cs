using FilePocket.BlazorClient.Features.Files.Models;
using FilePocket.BlazorClient.Features.SharedFiles.Models;
using FilePocket.BlazorClient.Features.SharedFiles.Requests;
using FilePocket.BlazorClient.Features.Storage.Models;
using FilePocket.BlazorClient.Features.Storage.Requests;
using FilePocket.BlazorClient.Services.Files.Models;
using FilePocket.BlazorClient.Services.Files.Requests;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace FilePocket.BlazorClient.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject]
        private IFileRequests FileRequests { get; set; } = default!;

        [Inject]
        private ISharedFilesRequests SharedFilesRequests { get; set; } = default!;
        [Inject]
        private IStorageRequests StorageRequests { get; set; } = default!;

        private List<SharedFileView>? _sharedFiles;

        private ObservableCollection<FileInfoModel> _files = new();

        private StorageConsumptionModel _storageConsumption = new();

        private string _storageCapacity = "";

        private bool _areParametrsReady = false;

        protected override async Task OnInitializedAsync()
        {
            var files = await FileRequests.GetRecentFilesAsync();

            if (files.Any())
            {
                _files = new ObservableCollection<FileInfoModel>(files);
            }

            _sharedFiles = await SharedFilesRequests.GetLatestAsync();

            _storageConsumption = await StorageRequests.GetStorageConsumption();
            GetStorageCapacity();
            _areParametrsReady = true;
        }

        public static FileTypes ParseEnum(string value)
        {
            return (FileTypes)Enum.Parse(typeof(FileTypes), value, true);
        }

        private void GetStorageCapacity()
        {
            _storageCapacity = Math.Round(Convert.ToDecimal((1 - (_storageConsumption.Used / _storageConsumption.Total)) * 100), 1)
                .ToString()
                .Replace(',', '.');
        }
    }
}

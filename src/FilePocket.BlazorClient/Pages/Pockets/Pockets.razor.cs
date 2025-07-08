using FilePocket.BlazorClient.Features.Trash.Requests;
using FilePocket.BlazorClient.Services.Pockets.Models;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.BlazorClient.Pages.Pockets
{
    public partial class Pockets : ComponentBase
    {
        private List<PocketModel> _pockets = new List<PocketModel>();
        private Guid _pocketIdToBeChanged;
        private bool _removalProcessStarted = false;
        private bool _loading = true;

        [Inject]
        private IPocketRequests PocketRequests { get; set; } = default!;

        [Inject]
        private ITrashRequests TrashRequests { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _pockets = await GetAllCustomPockets();
            _loading = false;
        }

        private async Task<List<PocketModel>> GetAllCustomPockets()
        {
            var pockets = await PocketRequests.GetAllCustomAsync();

            return pockets.ToList();
        }

        private void RemoveClicked(PocketModel pocket)
        {
            _removalProcessStarted = true;
            _pocketIdToBeChanged = pocket.Id;
        }

        private void GoToPocket(Guid pocketId)
        {
            Navigation.NavigateTo($"/pockets/{pocketId}/files");
        }

        private void ShowDetailsClicked(PocketModel pocket)
        {
            Navigation.NavigateTo($"/pockets/{pocket.Id}/info");
        }

        private async void ConfirmDeletionClicked()
        {
            var pocket = _pockets.FirstOrDefault(c => c.Id == _pocketIdToBeChanged);

            if (pocket is not null)
            {
                _pockets.Remove(pocket);
                _removalProcessStarted = false;

                _ = await PocketRequests.MoveToTrashAsync(_pocketIdToBeChanged);

                _pocketIdToBeChanged = default;
            }
        }

        private void CancelDeletionClicked()
        {
            _removalProcessStarted = false;
            _pocketIdToBeChanged = default;
        }

        private string GetNumberOfFiles(int? number)
        {
            return number is null ? "0" : number.Value.ToString();
        }
    }
}

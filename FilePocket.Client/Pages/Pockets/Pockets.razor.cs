using FilePocket.Client.Services.Pockets.Models;
using FilePocket.Client.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages.Pockets
{
    public partial class Pockets
    {
        private List<PocketModel> _pockets = default!;
        
        private Guid _pocketIdToBeChanged;

        private bool _removalProcessStarted = false;
        
        [Inject] private IPocketRequests PocketRequests { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            _pockets = await GetAllPockets();
        }

        private async Task<List<PocketModel>> GetAllPockets()
        {
            //temp solution
            var userId = Guid.Parse("a9b78973-6458-498f-a313-ae26e56d223c");

            var pockets = await PocketRequests.GetAllAsync(userId);

            return pockets.ToList();
        }

        private void RemoveClicked(PocketModel pocket)
        {
            _removalProcessStarted = true;
            _pocketIdToBeChanged = pocket.Id;
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

                await PocketRequests.DeleteAsync(_pocketIdToBeChanged);
                _pocketIdToBeChanged = default;
            }
        }

        private void CancelDeletionClicked()
        {
            _removalProcessStarted = false;
            _pocketIdToBeChanged = default;
        }
    }
}

using FilePocket.Client.Features.Users.Models;
using FilePocket.Client.Features.Users.Requests;
using FilePocket.Client.Services.Pockets.Models;
using FilePocket.Client.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FilePocket.Client.Pages.Pockets
{
    public partial class Pockets
    {
        private List<PocketModel> _pockets = new List<PocketModel>();
        private Guid _pocketIdToBeChanged;
        private bool _removalProcessStarted = false;
        private LoggedInUserModel? _user;
        private string _userName = string.Empty;
        private bool _loading = true;
        [Inject] IUserRequests UserRequests { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] private IPocketRequests PocketRequests { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            _userName = user.Identity?.Name!;
            _user = await UserRequests.GetByUserNameAsync(_userName);

            if (_user == null) return;

            _pockets = await GetAllPockets();
            _loading = false;
        }
        
        private async Task<List<PocketModel>> GetAllPockets()
        {
            var userId = _user!.Id!.Value;

            var pockets = await PocketRequests.GetAllAsync(userId);

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

                await PocketRequests.DeleteAsync(_pocketIdToBeChanged);
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

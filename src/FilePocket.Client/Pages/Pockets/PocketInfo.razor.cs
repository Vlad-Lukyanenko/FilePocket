using FilePocket.Client.Features.Users.Models;
using FilePocket.Client.Features.Users.Requests;
using FilePocket.Client.Helpers;
using FilePocket.Client.Services.Pockets.Models;
using FilePocket.Client.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace FilePocket.Client.Pages.Pockets
{
    public partial class PocketInfo
    {
        [Parameter]
        public string PocketIdParam { get; set; } = string.Empty;
        private Guid _pocketId => Guid.Parse(PocketIdParam);
        private LoggedInUserModel? _user;
        private string _userName = string.Empty;

        [Inject] IUserRequests UserRequests { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private IPocketRequests PocketRequests { get; set; } = default!;

        private bool _updateProcessStarted = false;
        private bool _updateDescriptionProcessStarted = false;
        private string _oldPocketDescription = string.Empty;
        private string _newPocketDescription = string.Empty;
        private string _oldPocketName = string.Empty;
        private string _newPocketName = string.Empty;

        private PocketModel? _pocketInfo;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            _userName = user.Identity?.Name!;
            _user = await UserRequests.GetByUserNameAsync(_userName);

            if (_user == null) return;

            _pocketInfo = await PocketRequests.GetInfoAsync(_pocketId);
            _pocketInfo.Id = _pocketId;
            _pocketInfo.UserId = _user!.Id!.Value; // Guid.Parse("a9b78973-6458-498f-a313-ae26e56d223c");
        }

        private async void CopyIdToClipboard()
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", _pocketInfo!.Id);
        }

        private void UpdateClicked()
        {
            _updateProcessStarted = true;
            _newPocketName = _pocketInfo.Name;
            _oldPocketName = _pocketInfo.Name;
        }

        private async void ConfirmUpdateClicked()
        {
            if (string.IsNullOrWhiteSpace(_newPocketName))
            {
                return;
            }

            if (_pocketInfo is not null && _newPocketName != _oldPocketName)
            {
                _pocketInfo.Name = _newPocketName;
                _updateProcessStarted = false;

                await PocketRequests.UpdateAsync(_pocketInfo);
            }

            _updateProcessStarted = false;
            _oldPocketName = string.Empty;
            _newPocketName = string.Empty;
        }

        private void CancelUpdateClicked()
        {
            _updateProcessStarted = false;
            _oldPocketName = string.Empty;
            _newPocketName = string.Empty;
        }
        private async Task UpdateDescriptionClicked()
        {
            _updateDescriptionProcessStarted = true;
            _newPocketDescription = _pocketInfo.Description;
            _oldPocketDescription = _pocketInfo.Description;

        }
        private async Task ConfirmDescriptionUpdateClicked()
        {
            if (string.IsNullOrWhiteSpace(_newPocketDescription) || _newPocketDescription.Length > Tools.MaxDescriptionLength)
            {
                return;
            }

            if (_pocketInfo is not null && _newPocketDescription != _oldPocketDescription)
            {
                _pocketInfo.Description = _newPocketDescription;
                _updateDescriptionProcessStarted = false;

                await PocketRequests.UpdateAsync(_pocketInfo);
            }

            _updateDescriptionProcessStarted = false;
            _oldPocketDescription = string.Empty;
            _newPocketDescription = string.Empty;
        }
        private void CancelDescriptionUpdateClicked()
        {
            _updateDescriptionProcessStarted = false;
        }
    }
}

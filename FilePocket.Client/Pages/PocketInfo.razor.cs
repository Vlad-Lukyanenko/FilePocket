using FilePocket.Client.Services.Pockets.Models;
using FilePocket.Client.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FilePocket.Client.Pages
{
    public partial class PocketInfo
    {
        [Parameter]
        public string PocketIdParam { get; set; } = string.Empty;

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        private Guid _pocketId => Guid.Parse(PocketIdParam);

        [Inject] private IPocketRequests PocketRequests { get; set; } = default!;

        private bool _updateProcessStarted = false;
        private string _oldPocketName = string.Empty;
        private string _newPocketName = string.Empty;

        private PocketModel? _pocketInfo;
       
        protected override async Task OnInitializedAsync()
        {
            _pocketInfo = await PocketRequests.GetDetails(_pocketId);
            _pocketInfo.Id = _pocketId;
            _pocketInfo.UserId = Guid.Parse("a9b78973-6458-498f-a313-ae26e56d223c");
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
    }
}

using FilePocket.Client.Services.Pockets.Models;
using FilePocket.Client.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;

namespace FilePocket.Client.Pages.Pockets
{
    public partial class CreatePocket
    {
        private string _pocketName = string.Empty;
        private bool _validName = true;

        [Inject] private IPocketRequests PocketRequests { get; set; } = default!;

        private async Task CreatePocketAsync()
        {
            if (string.IsNullOrEmpty(_pocketName))
            {
                _validName = false;
            }

            var model = new CreatePocketModel()
            {
                Name = _pocketName,
                UserId = Guid.Parse("a9b78973-6458-498f-a313-ae26e56d223c")
            };

            var result = await PocketRequests.CreateAsync(model);

            if (result)
            {
                Navigation.NavigateTo("/pockets");
            }
        }

        private void NameChanged()
        {
            _validName = !string.IsNullOrEmpty(_pocketName);
        }
    }
}

using FilePocket.Client.Features.Users.Models;
using FilePocket.Client.Features.Users.Requests;
using FilePocket.Client.Services.Pockets.Models;
using FilePocket.Client.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FilePocket.Client.Pages.Pockets
{
    public partial class CreatePocket
    {
        private const int MaxDescriptionLength = 500;
        private string _pocketName = string.Empty;
        private string _pocketDescription = string.Empty;
        private bool _validName = true;
        private bool _validDescription = true;
        private LoggedInUserModel? _user;
        private string _userName = string.Empty;

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
        }

        private async Task CreatePocketAsync()
        {
            if (string.IsNullOrEmpty(_pocketName))
            {
                _validName = false;
            }

            var model = new CreatePocketModel()
            {
                Name = _pocketName,
                Description = _pocketDescription,
                UserId =  _user!.Id!.Value //Guid.Parse("a9b78973-6458-498f-a313-ae26e56d223c")// 
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
        private void DescriptionChanged()
        {
<<<<<<< Updated upstream
            _validDescription = !string.IsNullOrEmpty(_pocketDescription);
=======
            _validDescription = !string.IsNullOrWhiteSpace(_pocketDescription);
>>>>>>> Stashed changes
        }
    }
}

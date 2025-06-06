﻿using FilePocket.BlazorClient.Features.Users.Models;
using FilePocket.BlazorClient.Features.Users.Requests;
using FilePocket.BlazorClient.Helpers;
using FilePocket.BlazorClient.Services.Pockets.Models;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FilePocket.BlazorClient.Pages.Pockets
{
    public partial class CreatePocket : ComponentBase
    {
        private string _pocketName = string.Empty;
        private string _pocketDescription = string.Empty;
        private bool _validName = true;
        private bool _validDescription = true;
        private LoggedInUserModel? _user;
        private string _userName = string.Empty;

        [Inject] 
        IUserRequests UserRequests { get; set; } = default!;

        [Inject] 
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject] 
        private IPocketRequests PocketRequests { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

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
            _validDescription = !string.IsNullOrWhiteSpace(_pocketDescription) && _pocketDescription.Length <= Tools.MaxDescriptionLength;
        }
    }
}

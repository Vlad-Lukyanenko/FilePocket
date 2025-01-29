﻿using FilePocket.Client.Features.SharedFiles.Models;
using FilePocket.Client.Features.SharedFiles.Requests;
using FilePocket.Client.Features.Users.Requests;
using FilePocket.Client.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FilePocket.Client.Pages.Files.Shared
{
    public partial class SharedFile
    {
        [Parameter]
        public string SharedFileId { get; set; } = string.Empty;

        [Inject]
        private ISharedFilesRequests _sharedFilesRequests { get; set; } = default!;
        
        [Inject] 
        private IUserRequests _userRequests { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private string _userName = string.Empty;
        
        private Guid _sharedFileId;
        private SharedFileModel? _sharedFile = null;

        private bool _showLoader = false;
        private bool _fileNotFound = false;

        protected override async Task OnInitializedAsync()
        {
            _sharedFileId = Guid.Parse(SharedFileId);

            _sharedFile = await _sharedFilesRequests.GetByIdAsync(_sharedFileId);

            if (_sharedFile is null)
            {
                _fileNotFound = true;
                return;
            }

            _userName = $"{_sharedFile.FirstName} {_sharedFile.LastName}";            
        }

        private string GetUserName()
        {
            return string.IsNullOrWhiteSpace(_userName) ? "Someone" : _userName;
        }

        private async Task DownloadFile()
        {
            _showLoader = true;
            StateHasChanged();

            var file = await _sharedFilesRequests.DownloadAsync(_sharedFileId);

            if(file is null) { return; }

            var base64 = Convert.ToBase64String(file);
            var mimeType = Tools.GetMimeType(_sharedFile.FileName);
            await JSRuntime.InvokeVoidAsync("saveFile", _sharedFile.FileName, mimeType, base64);

            _showLoader = false;
            StateHasChanged();
        }
    }
}

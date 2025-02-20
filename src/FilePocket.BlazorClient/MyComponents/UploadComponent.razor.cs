using FilePocket.BlazorClient.Services.Files.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FilePocket.BlazorClient.MyComponents;

public partial class UploadComponent : ComponentBase
{
    [Inject]
    public FilePocketBirdClient PocketTusClient { get; set; }

    [Parameter]
    public EventCallback<string> OnUploadComplete { get; set; }

    private const int MaxAllowedFiles = 15;
    private const int MaxFileSize = 1024 * 1024 * 1024;
    private async void UploadFileAsync(InputFileChangeEventArgs e)
    {
        var files = e.GetMultipleFiles(MaxAllowedFiles);
        if (!files.Any())
        {
            Console.WriteLine("No files to process.");
            return;
        }
        
        foreach (var file in files)
        {
            var uploadFileInfo = new FileInfoModel()
            {
                OriginalName = file.Name,
                TotalBytes = file.Size
            }; 
            
         _files.Add(uploadFileInfo);

        try
        {
            // Step 1: Create Upload Session
            var createResponse = await PocketTusClient.CreateUploadSessionAsync(file.Size, file.Name);

            // Step 2: Start Upload with Progress Tracking
            await using var stream = file.OpenReadStream();
            var success = await PocketTusClient.UploadFileAsync(createResponse, stream, (uploaded, total) =>
            {
                uploadFileInfo.UploadedBytes = uploaded;
                InvokeAsync(StateHasChanged);
            }, _cts.Token);

            if (!success)
            {
                Console.WriteLine($"Upload failed for {file.Name}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file {file.Name}: {ex.Message}");
        }
        }
    }
}
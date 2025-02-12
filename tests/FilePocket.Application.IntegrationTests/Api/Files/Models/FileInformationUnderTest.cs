using FilePocket.WebApi.Controllers;

namespace FilePocket.Application.IntegrationTests.Api.Files.Models;

/// <summary>
/// To scrape the data from the files to be uploaded, and use it for assertions.
/// </summary>
public class FileInformationUnderTest : FilesController.FileInformation
{
    public Guid UserId { get; init; }
    public string FileName { get; init; }
    public string FileExtension { get; init; }
    public long FileSizeInBytes { get; init; }
    public MultipartFormDataContent MultipartFormDataContent { get; init; }
    
    public string GetOriginalFileName() => $"{FileName}.{FileExtension}";
}
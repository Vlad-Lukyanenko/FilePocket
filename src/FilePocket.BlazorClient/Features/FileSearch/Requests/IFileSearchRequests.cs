using FilePocket.BlazorClient.Features.FileSearch.Models;

namespace FilePocket.BlazorClient.Features.FileSearch.Requests
{
    public interface IFileSearchRequests
    {
        Task<List<FileSearchResponseModel>> GetFilesByPartialNameAsync(string partialNameToSearch);
    }
}

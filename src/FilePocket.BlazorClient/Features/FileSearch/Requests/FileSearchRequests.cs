using FilePocket.BlazorClient.Features.FileSearch.Models;
using Newtonsoft.Json;

namespace FilePocket.BlazorClient.Features.FileSearch.Requests
{
    public class FileSearchRequests : IFileSearchRequests
    {
        private readonly FilePocketApiClient _apiClient;

        public FileSearchRequests(FilePocketApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<FileSearchResponseModel>> GetFilesByPartialNameAsync(string partialNameToSearch)
        {
            var url = FileSearchUrl.GetFilesByPartialName(partialNameToSearch);

            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<FileSearchResponseModel>>(content)!;
        }
    }
}

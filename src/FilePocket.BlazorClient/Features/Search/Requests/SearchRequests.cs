﻿using FilePocket.BlazorClient.Features.Search.Enums;
using Newtonsoft.Json;

namespace FilePocket.BlazorClient.Features.Search.Requests
{
    public class SearchRequests : ISearchRequests
    {
        private readonly FilePocketApiClient _apiClient;

        public SearchRequests(FilePocketApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<T>> GetItemsByPartialNameAsync<T>(SearchItemType itemType, string partialNameToSearch)
        {
            var url = SearchUrl.GeItemsByPartialName(itemType, partialNameToSearch);
            var content = await _apiClient.GetAsync(url);

            return JsonConvert.DeserializeObject<List<T>>(content)!;
        }
    }
}

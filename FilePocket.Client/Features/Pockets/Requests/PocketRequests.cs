using Blazored.LocalStorage;
using FilePocket.Client.Services.Pockets.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FilePocket.Client.Services.Pockets.Requests
{
    public class PocketRequests : IPocketRequests
    {
        private const string HttpClientName = "FilePocketApi";

        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public PocketRequests(IHttpClientFactory factory, ILocalStorageService localStorage)
        {
            _httpClient = factory.CreateClient(HttpClientName);
            _localStorage = localStorage; 
        }

        public async Task<IEnumerable<PocketModel>> GetAllAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync(PocketUrl.GetAll(userId));

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<PocketModel>>(content)!;
        }

        public async Task<PocketModel> GetInfoAsync(Guid pocketId)
        {
            var url = PocketUrl.GetInfo(pocketId);
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<PocketModel>(content)!;
        }

        public async Task<bool> CreateAsync(CreatePocketModel pocket)
        {
            var content = GetStringContent(pocket);

            var response = await _httpClient.PostAsync(PocketUrl.BaseUrl, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(PocketModel pocket)
        {
            var content = GetStringContent(pocket);

            var response = await _httpClient.PutAsync(PocketUrl.Update(pocket.Id), content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid pocketId)
        {
            var response = await _httpClient.DeleteAsync(PocketUrl.Update(pocketId));

            return response.IsSuccessStatusCode;
        }

        public Task<bool> CheckPocketCapacityAsync(Guid pocket, long fileSize)
        {
            throw new NotImplementedException();
        }

        private static StringContent? GetStringContent(object? obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}

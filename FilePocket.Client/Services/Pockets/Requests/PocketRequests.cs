using FilePocket.Client.Services.Pockets.Models;
using Newtonsoft.Json;
using System.Text;

namespace FilePocket.Client.Services.Pockets.Requests
{
    public class PocketRequests : IPocketRequests
    {
        private const string HttpClientName = "FilePocketApi";

        private readonly HttpClient _httpClient;

        public PocketRequests(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient(HttpClientName);
        }

        public async Task<IEnumerable<PocketModel>> GetAllAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync(PocketUrl.GetAll(userId));

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<IEnumerable<PocketModel>>(content)!;
        }

        public async Task<PocketModel> GetDetails(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/storages/info/{id}");

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<PocketModel>(content)!;
        }

        public Task<bool> CheckPocketCapacity(Guid pocket, long fileSize)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateAsync(CreatePocketModel pocket)
        {
            var json = JsonConvert.SerializeObject(pocket);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/storages/", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/storages/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(PocketModel pocket)
        {
            var json = JsonConvert.SerializeObject(pocket);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/storages/{pocket.Id}", content);

            return response.IsSuccessStatusCode;
        }
    }
}

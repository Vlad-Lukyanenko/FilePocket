using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FilePocket.Client.Features
{
    public class FilePocketApiClient
    {
        private readonly HttpClient _httpClient;

        public FilePocketApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7003");
        }

        public Task<HttpResponseMessage> PostAsJsonAsync(string endpoint, object model)
        {
            return _httpClient.PostAsJsonAsync(endpoint, model);
        }

        public Task<HttpResponseMessage> PostAsync(string endpoint, StringContent? content)
        {
            return _httpClient.PostAsync(endpoint, content);
        }
        
        public Task<HttpResponseMessage> PostAsync(string endpoint, MultipartFormDataContent? content)
        {
            return _httpClient.PostAsync(endpoint, content);
        }

        public Task<HttpResponseMessage> PutAsync(string endpoint, StringContent? content)
        {
            return _httpClient.PutAsync(endpoint, content);
        }

        public Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            return _httpClient.DeleteAsync(endpoint);
        }

        public async Task<string> GetAsync(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);

            return await response.Content.ReadAsStringAsync();
        }

        public void SetBearerAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void CleanUpAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}

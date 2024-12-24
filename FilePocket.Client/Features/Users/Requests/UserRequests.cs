using FilePocket.Client.Features;
using FilePocket.Client.Features.Users.Models;
using Newtonsoft.Json;

namespace FilePocket.Client.Features.Users.Requests
{
    public class UserRequests : IUserRequests
    {
        private readonly FilePocketApiClient _apiClient;

        public UserRequests(FilePocketApiClient authorizedRequests)
            => _apiClient = authorizedRequests;

        public async Task<LoggedInUserModel?> GetByUserNameAsync(string userName)
        {

            var response = await _apiClient.GetAsync($"api/users/{userName}");

            return JsonConvert.DeserializeObject<LoggedInUserModel>(response);
        }
    }
}

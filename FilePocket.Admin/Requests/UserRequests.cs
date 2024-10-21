using FilePocket.Admin.Models;
using FilePocket.Admin.Requests.Contracts;
using FilePocket.Admin.Requests.HttpRequests;
using Newtonsoft.Json;

namespace FilePocket.Admin.Requests
{
    public class UserRequests : IUserRequests
    {
        private readonly IHttpRequests _authorizedRequests = default!;

        public UserRequests(IHttpRequests authorizedRequests)
            => _authorizedRequests = authorizedRequests;

        public async Task<LoggedInUserModel?> GetByUserNameAsync(string userName)
        {

            var response = await _authorizedRequests.GetAsyncRequest($"api/users/{userName}");
            var content = await response.Content.ReadAsStringAsync();

            if (content == null) return null;

            return JsonConvert.DeserializeObject<LoggedInUserModel>(content)!;
        }
    }
}

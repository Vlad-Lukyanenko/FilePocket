using FilePocket.BlazorClient.Features.HtmlParser.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace FilePocket.BlazorClient.Features.HtmlParser.Requests
{
    public class HtmlParserService : IHtmlParserService
    {
        private readonly FilePocketApiClient _apiClient;

        public HtmlParserService(FilePocketApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<WebsitePreviewModel> GetWebSitePreviewAsync(string websiteUri)
        {
            var queryStringParams = new Dictionary<string, string>
            {
                ["siteUri"] = websiteUri ?? string.Empty
            };

            var websitePreview = await _apiClient.GetFromJsonAsync<WebsitePreviewModel>(QueryHelpers.AddQueryString(HtmlParserUrl.WebsitePreview, queryStringParams!));

            
            return websitePreview;
        }
    }
}
